using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Split_It_.Model;
using System.Globalization;
using Split_It_.Utils;
using System.Windows.Media;
using System.Collections;
using Split_It_.Controller;
using System.Windows.Input;

namespace Split_It_.Add_Expense_Pages
{
    public partial class ExpenseUserControl : UserControl
    {
        public enum AmountSplit { You_owe, You_are_owed, Split_equally, Split_unequally };
        protected enum SplitUnequally { EXACT_AMOUNTS, PERCENTAGES, SHARES };

        public Expense expense;

        protected BackgroundWorker getSupportedCurrenciesBackgroundWorker;
        protected ObservableCollection<Currency> currenciesList = new ObservableCollection<Currency>();
        public ObservableCollection<Expense_Share> expenseShareUsers = new ObservableCollection<Expense_Share>();
        public AmountSplit amountSplit;
        protected SplitUnequally splitUnequally = SplitUnequally.EXACT_AMOUNTS;

        string decimalsep = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
        public static string EQUALLY = "split equally.";
        public static string UNEQUALLY = "split unequally.";
        public static string FULL_AMOUNT = "the full amount.";
        public static string OWES = "owes ";
        public static string OWE = "owe ";

        public ExpenseUserControl()
        {
            InitializeComponent();
            if (PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] != null)
                expense = PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] as Expense;

            getSupportedCurrenciesBackgroundWorker = new BackgroundWorker();
            getSupportedCurrenciesBackgroundWorker.WorkerSupportsCancellation = true;
            getSupportedCurrenciesBackgroundWorker.DoWork += new DoWorkEventHandler(getSupportedCurrenciesBackgroundWorker_DoWork);

            if (!getSupportedCurrenciesBackgroundWorker.IsBusy)
                getSupportedCurrenciesBackgroundWorker.RunWorkerAsync();

            (App.Current.Resources["PhoneRadioCheckBoxCheckBrush"] as SolidColorBrush).Color = Colors.Black;

            this.friendListPicker.ItemsSource = App.friendsList;
            this.friendListPicker.SummaryForSelectedItemsDelegate = this.FriendSummaryDelegate;

            this.groupListPicker.ItemsSource = App.groupsList;
            this.groupListPicker.SummaryForSelectedItemsDelegate = this.GroupSummaryDelegate;

            this.currencyListPicker.ItemsSource = this.currenciesList;
            this.currencyListPicker.SummaryForSelectedItemsDelegate = this.CurrencySummaryDelegate;

            if (expense == null)
                expense = new Expense();

            llsExactAmount.ItemsSource = expenseShareUsers;
            llsPercentage.ItemsSource = expenseShareUsers;
            llsShares.ItemsSource = expenseShareUsers;
        }

        private object FriendSummaryDelegate(IList list)
        {
            string summary = String.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                // check if the last item has been reached so we don't put a "," at the end
                bool isLast = i == list.Count - 1;

                User friend = (User)list[i];
                summary = String.Concat(summary, friend.first_name);
                summary += isLast ? string.Empty : ", ";
            }
            if (summary == String.Empty)
            {
                summary = "no friends selected";
            }
            return summary;
        }

        private object GroupSummaryDelegate(IList list)
        {
            string summary = String.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                // check if the last item has been reached so we don't put a "," at the end
                bool isLast = i == list.Count - 1;

                Group group = (Group)list[i];
                summary = String.Concat(summary, group.name);
                summary += isLast ? string.Empty : ", ";
            }
            if (summary == String.Empty)
            {
                summary = "select all members of group";
            }
            return summary;
        }

        public bool setupExpense()
        {
            //FocusedTextBoxUpdateSource();
            //to hide the keyboard if any
            this.Focus();
            bool proceed = true;

            switch (amountSplit)
            {
                case AmountSplit.You_owe:
                    divideExpenseYouOwe();
                    break;
                case AmountSplit.You_are_owed:
                    divideExpenseOwesYou();
                    break;
                case AmountSplit.Split_equally:
                    divideExpenseEqually();
                    break;
                case AmountSplit.Split_unequally:
                    proceed = divideExpenseUnequally();
                    break;
                default:
                    break;
            }
            expense.users = expenseShareUsers.ToList();
            expense.currency_code = (currencyListPicker.SelectedItem as Currency).currency_code;
            expense.payment = false;
            expense.details = tbDetails.Text;
            DateTime dateTime = expenseDate.Value ?? DateTime.Now;
            expense.date = dateTime.ToString("yyyy-MM-ddTHH:mm:ssK");

            return proceed;
        }
        
        protected void divideExpenseEqually()
        {
            int numberOfExpenseMembers = expenseShareUsers.Count;
            double amountToSplit = Convert.ToDouble(expense.cost);
            double perPersonShare = amountToSplit / numberOfExpenseMembers;
            perPersonShare = Math.Round(perPersonShare, 2, MidpointRounding.AwayFromZero);

            //now make sure that the total is the same, else take care of the difference (should be very minimal)
            double currentUsersShare = perPersonShare;
            double total = perPersonShare * numberOfExpenseMembers;
            if (total == amountToSplit)
                currentUsersShare = perPersonShare;
            else
                currentUsersShare = currentUsersShare + (amountToSplit - total);

            for (int i = 0; i < numberOfExpenseMembers; i++)
            {
                //the amount is paid by the user
                if (expenseShareUsers[i].user_id == App.currentUser.id)
                {
                    expenseShareUsers[i].paid_share = amountToSplit.ToString();
                    expenseShareUsers[i].owed_share = currentUsersShare.ToString();

                }
                else
                {
                    expenseShareUsers[i].paid_share = "0";
                    expenseShareUsers[i].owed_share = perPersonShare.ToString();
                }

                expenseShareUsers[i].share = 1;
            }
        }

        //This means that the other person paid and you owe the full amount
        protected void divideExpenseYouOwe()
        {
            //only you and one more user should be there to access this feature
            if (expenseShareUsers.Count != 2)
                throw new IndexOutOfRangeException();
            int numberOfExpenseMembers = expenseShareUsers.Count;
            for (int i = 0; i < numberOfExpenseMembers; i++)
            {
                //the amount is owed by the user
                if (expenseShareUsers[i].user_id == App.currentUser.id)
                {
                    expenseShareUsers[i].paid_share = "0";
                    expenseShareUsers[i].owed_share = tbAmount.Text;

                }
                else
                {
                    expenseShareUsers[i].paid_share = tbAmount.Text;
                    expenseShareUsers[i].owed_share = "0";
                }
            }
        }

        //This means that you paid and the other person owes you the full amount
        protected void divideExpenseOwesYou()
        {
            //only you and one more user should be there to access this feature
            if (expenseShareUsers.Count != 2)
                throw new IndexOutOfRangeException();

            int numberOfExpenseMembers = expenseShareUsers.Count;
            for (int i = 0; i < numberOfExpenseMembers; i++)
            {
                //the amount is paid by the user
                if (expenseShareUsers[i].user_id == App.currentUser.id)
                {
                    expenseShareUsers[i].owed_share = "0";
                    expenseShareUsers[i].paid_share = tbAmount.Text;

                }
                else
                {
                    expenseShareUsers[i].paid_share = "0";
                    expenseShareUsers[i].owed_share = tbAmount.Text;
                }
            }
        }

        protected bool divideExpenseUnequally()
        {
            bool proceed = true;
            double expenseAmount = Convert.ToDouble(expense.cost);
            int numberOfExpenseMembers = expenseShareUsers.Count;
            double extraAmount = 0; //this is extra amount left because of rounding off;
            int currentUserIndex = 0; //need the current user index to factor in the extra amount mentioned above
            double currenUserShareAmount = 0;

            switch (splitUnequally)
            {
                case SplitUnequally.EXACT_AMOUNTS:
                    double totalAmount = 0;

                    for (int i = 0; i < numberOfExpenseMembers; i++)
                    {
                        //the amount is paid by the user
                        if (expenseShareUsers[i].user_id == App.currentUser.id)
                        {
                            expenseShareUsers[i].paid_share = expenseAmount.ToString();
                        }
                        totalAmount += Convert.ToDouble(expenseShareUsers[i].owed_share);
                    }

                    if (totalAmount != expenseAmount)
                    {
                        string errorMessage = "The total amount split is not equal to expense amount.";
                        string newLine = "\n";
                        string total = "Input total: " + totalAmount.ToString();
                        string expenseTotal = "Expense Amount: " + expenseAmount.ToString();
                        MessageBox.Show(errorMessage + newLine + newLine + total + newLine + expenseTotal, "Error", MessageBoxButton.OK);

                        proceed = false;
                    }
                    break;
                case SplitUnequally.PERCENTAGES:
                    double totalPercentage = 0;

                    for (int i = 0; i < numberOfExpenseMembers; i++)
                    {
                        //the amount is paid by the user
                        if (expenseShareUsers[i].user_id == App.currentUser.id)
                        {
                            expenseShareUsers[i].paid_share = expenseAmount.ToString();
                            currentUserIndex = i;
                        }
                        double shareAmount = expenseAmount * expenseShareUsers[i].percentage / 100;

                        //round off amount to digits
                        double shareAmountRounded = Math.Round(shareAmount, 2, MidpointRounding.AwayFromZero);
                        extraAmount += shareAmount - shareAmountRounded;

                        expenseShareUsers[i].owed_share = shareAmountRounded.ToString();

                        totalPercentage += expenseShareUsers[i].percentage;
                    }

                    currenUserShareAmount = Convert.ToDouble(expenseShareUsers[currentUserIndex].owed_share);
                    currenUserShareAmount += extraAmount;
                    expenseShareUsers[currentUserIndex].owed_share = currenUserShareAmount.ToString();

                    if (totalPercentage != 100)
                    {
                        string errorMessage = "Total percentage is not equal to 100%";
                        MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK);

                        proceed = false;
                    }
                    break;
                case SplitUnequally.SHARES:
                    double totalShares = 0;

                    for (int i = 0; i < numberOfExpenseMembers; i++)
                    {
                        totalShares += expenseShareUsers[i].share;
                    }

                    double perShareAmount = expenseAmount / totalShares;
                    for (int i = 0; i < numberOfExpenseMembers; i++)
                    {
                        //the amount is paid by the user
                        if (expenseShareUsers[i].user_id == App.currentUser.id)
                        {
                            expenseShareUsers[i].paid_share = expenseAmount.ToString();
                            currentUserIndex = i;
                        }
                        double shareAmount = expenseShareUsers[i].share * perShareAmount;

                        //round off amount to digits
                        double shareAmountRounded = Math.Round(shareAmount, 2, MidpointRounding.AwayFromZero);
                        extraAmount += shareAmount - shareAmountRounded;

                        expenseShareUsers[i].owed_share = shareAmountRounded.ToString();
                        expenseShareUsers[i].owed_share = shareAmountRounded.ToString();
                    }

                    currenUserShareAmount = Convert.ToDouble(expenseShareUsers[currentUserIndex].owed_share);
                    currenUserShareAmount += extraAmount;
                    expenseShareUsers[currentUserIndex].owed_share = currenUserShareAmount.ToString();
                    break;
                default:
                    break;
            }

            return proceed;
        }

        protected void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.friendListPicker.SelectedItems.Count != 0 && !String.IsNullOrEmpty(tbAmount.Text))
            {
                toggle();
                showUnequalSectionIfNeeded();
            }

            else
            {
                MessageBox.Show("Please select friends and enter expense amount", "Error", MessageBoxButton.OK);
            }
        }

        private void toggle()
        {
            if (this.friendListPicker.SelectedItems.Count == 1)
            {
                User selectedUser = this.friendListPicker.SelectedItem as User;
                switch (amountSplit)
                {
                    //By default we have split equally, so follow the follwoing pattern
                    // a) If You owe then show stuff for you are owed
                    // b) If you are owed then show for split unequally
                    // c) If split equally then show stuff for you owe
                    // d) If split unequally then show stuff for split equally

                    case AmountSplit.You_owe:
                        tbPaidBy.Text = "You";
                        tbPaidTo.Text = selectedUser.first_name;
                        tbFullAmount.Text = OWES + FULL_AMOUNT;
                        amountSplit = AmountSplit.You_are_owed;
                        break;
                    case AmountSplit.You_are_owed:
                        tbPaidBy.Text = "You";
                        tbPaidTo.Text = UNEQUALLY;
                        tbFullAmount.Text = "";
                        amountSplit = AmountSplit.Split_unequally;
                        break;
                    case AmountSplit.Split_equally:
                        tbPaidBy.Text = selectedUser.first_name;
                        tbPaidTo.Text = "You";
                        tbFullAmount.Text = OWE + FULL_AMOUNT;
                        amountSplit = AmountSplit.You_owe;
                        break;
                    case AmountSplit.Split_unequally:
                        tbPaidBy.Text = "You";
                        tbPaidTo.Text = EQUALLY;
                        tbFullAmount.Text = "";
                        amountSplit = AmountSplit.Split_equally;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (amountSplit)
                {
                    case AmountSplit.Split_equally:
                        tbPaidBy.Text = "You";
                        tbPaidTo.Text = UNEQUALLY;
                        tbFullAmount.Text = "";
                        amountSplit = AmountSplit.Split_unequally;
                        break;
                    case AmountSplit.Split_unequally:
                        tbPaidBy.Text = "You";
                        tbPaidTo.Text = EQUALLY;
                        tbFullAmount.Text = "";
                        amountSplit = AmountSplit.Split_equally;
                        break;
                    default:
                        tbPaidBy.Text = "You";
                        tbPaidTo.Text = EQUALLY;
                        tbFullAmount.Text = "";
                        amountSplit = AmountSplit.Split_equally;
                        break;
                }
            }
        }

        public void showUnequalSectionIfNeeded()
        {
            if (amountSplit != AmountSplit.Split_unequally)
                spUnequally.Visibility = System.Windows.Visibility.Collapsed;
            else
            {
                divideExpenseEqually();
                spUnequally.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void getSupportedCurrenciesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Currency defaultCurrency = null;
            QueryDatabase query = new QueryDatabase();
            foreach (var item in query.getSupportedCurrencies())
            {
                if (item.currency_code == App.currentUser.default_currency && String.IsNullOrEmpty(expense.currency_code))
                    defaultCurrency = item;

                else if (item.currency_code == expense.currency_code)
                    defaultCurrency = item;

                Dispatcher.BeginInvoke(() =>
                {
                    currenciesList.Add(item);
                });
            }
            Dispatcher.BeginInvoke(() =>
            {
                this.currencyListPicker.SelectedItem = defaultCurrency;
            });
        }

        private object CurrencySummaryDelegate(IList list)
        {
            string summary = String.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                // check if the last item has been reached so we don't put a "," at the end
                bool isLast = i == list.Count - 1;

                Currency currency = (Currency)list[i];
                summary = String.Concat(summary, currency.currency_code);
                summary += isLast ? string.Empty : ", ";
            }
            if (String.IsNullOrEmpty(summary))
            {
                summary = "select currency";
            }
            return summary;
        }

        protected void tbAmount_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            PhoneTextBox textBox = sender as PhoneTextBox;

            //do not llow user to input more than one decimal point
            if (textBox.Text.Contains(decimalsep) && e.PlatformKeyCode == 190)
                e.Handled = true;
        }

        protected void tbUnequal_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            //do not llow user to input more than one decimal point
            if (textBox.Text.Contains(decimalsep) && e.PlatformKeyCode == 190)
                e.Handled = true;
        }

        protected void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton btn = sender as RadioButton;

            if (llsExactAmount == null || llsPercentage == null || llsShares == null)
                return;

            if (btn.Content.ToString() == "amount")
            {
                splitUnequally = SplitUnequally.EXACT_AMOUNTS;
                llsExactAmount.Visibility = System.Windows.Visibility.Visible;
                llsPercentage.Visibility = System.Windows.Visibility.Collapsed;
                llsShares.Visibility = System.Windows.Visibility.Collapsed;
            }

            else if (btn.Content.ToString() == "percentage")
            {
                splitUnequally = SplitUnequally.PERCENTAGES;
                llsExactAmount.Visibility = System.Windows.Visibility.Collapsed;
                llsPercentage.Visibility = System.Windows.Visibility.Visible;
                llsShares.Visibility = System.Windows.Visibility.Collapsed;
            }

            else if (btn.Content.ToString() == "share")
            {
                splitUnequally = SplitUnequally.SHARES;
                llsExactAmount.Visibility = System.Windows.Visibility.Collapsed;
                llsPercentage.Visibility = System.Windows.Visibility.Collapsed;
                llsShares.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public static void FocusedTextBoxUpdateSource()
        {
            var focusedElement = FocusManager.GetFocusedElement();
            var focusedTextBox = focusedElement as TextBox;

            if (focusedTextBox != null)
            {
                var binding = focusedTextBox.GetBindingExpression(TextBox.TextProperty);

                if (binding != null)
                {
                    binding.UpdateSource();
                }
            }
            else
            {
                var focusedPasswordBox = focusedElement as PasswordBox;

                if (focusedPasswordBox != null)
                {
                    var binding = focusedPasswordBox.GetBindingExpression(PasswordBox.PasswordProperty);

                    if (binding != null)
                    {
                        binding.UpdateSource();
                    }
                }
            }
        }
    }
}
