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

namespace Split_It_.UserControls
{
    public partial class ExpenseUserControl : UserControl
    {
        protected enum SplitUnequally { EXACT_AMOUNTS, PERCENTAGES, SHARES };

        public Expense expense;

        private BackgroundWorker getSupportedCurrenciesBackgroundWorker;
        
        public ObservableCollection<Expense_Share> friends = new ObservableCollection<Expense_Share>();
        public ObservableCollection<Group> groupsList = new ObservableCollection<Group>();

        private ObservableCollection<Currency> currenciesList = new ObservableCollection<Currency>();
        private ObservableCollection<Expense_Share> expenseShareUsers = new ObservableCollection<Expense_Share>();
        
        //By default the amount is split equally
        public AmountSplit amountSplit = AmountSplit.EqualSplit;
        protected SplitUnequally splitUnequally = SplitUnequally.EXACT_AMOUNTS;

        string decimalsep = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;

        public ExpenseUserControl()
        {
            InitializeComponent();
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                return;
            }

            loadFriendsAndGroups();
            if (PhoneApplicationService.Current.State.ContainsKey(Constants.ADD_EXPENSE) && PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] != null)
                expense = PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] as Expense;

            getSupportedCurrenciesBackgroundWorker = new BackgroundWorker();
            getSupportedCurrenciesBackgroundWorker.WorkerSupportsCancellation = true;
            getSupportedCurrenciesBackgroundWorker.DoWork += new DoWorkEventHandler(getSupportedCurrenciesBackgroundWorker_DoWork);

            if (!getSupportedCurrenciesBackgroundWorker.IsBusy)
                getSupportedCurrenciesBackgroundWorker.RunWorkerAsync();

            (App.Current.Resources["PhoneRadioCheckBoxCheckBrush"] as SolidColorBrush).Color = Colors.Black;

            this.friendListPicker.ItemsSource = friends;
            this.friendListPicker.SummaryForSelectedItemsDelegate = this.FriendSummaryDelegate;
            this.friendListPicker.SelectionChanged += friendListPicker_SelectionChanged;

            this.groupListPicker.ItemsSource = groupsList;
            this.groupListPicker.SummaryForSelectedItemsDelegate = this.GroupSummaryDelegate;

            if (groupsList == null || groupsList.Count == 0)
                this.groupListPicker.Visibility = System.Windows.Visibility.Collapsed;

            this.currencyListPicker.ItemsSource = currenciesList;
            this.SplitTypeListPicker.ItemsSource = AmountSplit.GetAmountSplitTypes();

            this.expenseTypeListPicker.SelectionChanged += expenseTypeListPicker_SelectionChanged;

            if (expense == null)
            {
                expense = new Expense();
                expense.group_id = 0; //by default the expense is in no group
            }

            //add current user
            expenseShareUsers.Add(getCurrentUser());
        }

        private Expense_Share getCurrentUser()
        {
            return new Expense_Share() { user = App.currentUser, user_id = App.currentUser.id };
        }

        private void loadFriendsAndGroups()
        {
            loadGroups();
            loadFriends();
        }

        private void loadGroups()
        {
            QueryDatabase obj = new QueryDatabase();
            List<Group> allGroups = obj.getAllGroups();
            
            if (allGroups != null && allGroups.Count!=0)
            {
                this.groupListPicker.Visibility = System.Windows.Visibility.Visible;
                foreach (var group in allGroups)
                {
                    groupsList.Add(group);
                }
            }

            obj.closeDatabaseConnection();
        }

        private void loadFriends()
        {
            QueryDatabase obj = new QueryDatabase();
            List<User> allFriends = obj.getAllFriends();
            if (allFriends != null)
            {
                foreach (var friend in allFriends)
                {
                    friends.Add(new Expense_Share() { user = friend, user_id = friend.id });
                }
            }
            obj.closeDatabaseConnection();
        }

#region Event Handlers

        private object FriendSummaryDelegate(IList list)
        {
            string summary = String.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                // check if the last item has been reached so we don't put a "," at the end
                bool isLast = i == list.Count - 1;

                Expense_Share friend = (Expense_Share)list[i];
                summary = String.Concat(summary, friend.user.first_name);
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

        private object PaidBySummaryDelegate(IList list)
        {
            String summary = "no friends selected";
            
            if (list.Count > 1)
                summary =  "Multiple Friends";
            
            if(list.Count == 1)
            {
                Expense_Share friend = (Expense_Share)list[0];
                summary = friend.user.first_name;
            }
            return summary;
        }

        private void getSupportedCurrenciesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Currency defaultCurrency = null;
            QueryDatabase query = new QueryDatabase();
            foreach (var item in query.getSupportedCurrencies())
            {
                if (item.currency_code == App.currentUser.default_currency && String.IsNullOrEmpty(expense.currency_code))
                    defaultCurrency = item;

                else if (!String.IsNullOrEmpty(expense.currency_code))
                {
                    if (item.currency_code == expense.currency_code)
                        defaultCurrency = item;
                }

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

        protected void tbAmount_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            PhoneTextBox textBox = sender as PhoneTextBox;

            //do not allow user to input more than one decimal point
            if (textBox.Text.Contains(decimalsep) && (e.PlatformKeyCode == 190 || e.PlatformKeyCode == 188))
                e.Handled = true;
        }

        protected void tbUnequal_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            //do not llow user to input more than one decimal point
            if (textBox.Text.Contains(decimalsep) && (e.PlatformKeyCode == 190 || e.PlatformKeyCode == 188))
                e.Handled = true;
        }

        /*protected void RadioButton_Checked(object sender, RoutedEventArgs e)
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
        }*/

        void friendListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int count = friendListPicker.SelectedItems.Count;

            if (count == 1)
            {
                Expense_Share selectedFriend = (Expense_Share)friendListPicker.SelectedItem;
                this.expenseTypeListPicker.ItemsSource = ExpenseType.GetAllExpenseTypeList(selectedFriend.user.first_name);
                this.expenseTypeListPicker.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.expenseTypeListPicker.ItemsSource = ExpenseType.GetOnlySplitExpenseTypeList();
                this.expenseTypeListPicker.Visibility = System.Windows.Visibility.Collapsed;
            }

            expenseShareUsers.Clear();
            //add current user
            expenseShareUsers.Add(getCurrentUser());
            foreach (var item in friendListPicker.SelectedItems)
            {
                expenseShareUsers.Add(item as Expense_Share);
            }
        }

        void expenseTypeListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpenseType type = (ExpenseType)this.expenseTypeListPicker.SelectedItem;
            if (type.id != ExpenseType.TYPE_SPLIT_BILL)
                this.paidByContainer.Visibility = System.Windows.Visibility.Collapsed;
            else
                this.paidByContainer.Visibility = System.Windows.Visibility.Visible;
        }

        /*void splitMethodListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            amountSplit = (AmountSplit) this.splitMethodListPicker.SelectedItem;
            showUnequalSectionIfNeeded();
        }*/
        
#endregion
        
        /*
         * Handles the following:
         * 1) Check if amount and description is present
         * 2) Check if Paid by user is selected
         * 3) Check if multiple paid by user then total amount = sum of amount by each paid by user
         * 4) Count the number of users who have paid (using hasPaid in expense_share). If count = 0, then current user has paid. Else the paid part is already settled for by the pop-up box
         * 4) If split unequally then, total amount should be split uneqaully amongst all of expenseShareUsers.
         */
        public bool setupExpense()
        {
            try
            {
                FocusedTextBoxUpdateSource();
                //to hide the keyboard if any
                this.Focus();
                bool proceed = true;

                //Check if description and amount are present.
                expense.description = tbDescription.Text;
                expense.cost = tbAmount.Text;
                if (String.IsNullOrEmpty(expense.cost) || String.IsNullOrEmpty(expense.description))
                {
                    MessageBox.Show("Please enter amount and description", "Error", MessageBoxButton.OK);
                    return false;
                }

                /*switch (amountSplit.id)
                {
                    case AmountSplit.YOU_OWE:
                        divideExpenseYouOwe();
                        break;
                    case AmountSplit.FRIEND_OWES:
                        divideExpenseOwesYou();
                        break;
                    case AmountSplit.SPLIT_EQUALLY:
                        divideExpenseEqually();
                        break;
                    case AmountSplit.SPLIT_UNEQUALLY:
                        proceed = divideExpenseUnequally();
                        break;
                    default:
                        break;
                }*/

                expense.users = expenseShareUsers.ToList();
                Currency selectedCurrency = (currencyListPicker.SelectedItem as Currency);

                if(selectedCurrency!=null)
                    expense.currency_code = selectedCurrency.currency_code;
                
                expense.payment = false;
                expense.details = tbDetails.Text;
                DateTime dateTime = expenseDate.Value ?? DateTime.Now;
                expense.date = dateTime.ToString("yyyy-MM-ddTHH:mm:ssK");

                return proceed;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void divideExpenseEqually()
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
                    //expenseShareUsers[i].paid_share = amountToSplit.ToString();
                    expenseShareUsers[i].owed_share = currentUsersShare.ToString();

                }
                else
                {
                    //expenseShareUsers[i].paid_share = "0";
                    expenseShareUsers[i].owed_share = perPersonShare.ToString();
                }

                expenseShareUsers[i].share = 1;
            }
        }

        //This means that the other person paid and you owe the full amount
        private void divideExpenseYouOwe()
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
                    //expenseShareUsers[i].paid_share = "0";
                    expenseShareUsers[i].owed_share = tbAmount.Text;

                }
                else
                {
                    //expenseShareUsers[i].paid_share = tbAmount.Text;
                    expenseShareUsers[i].owed_share = "0";
                }
            }
        }

        //This means that you paid and the other person owes you the full amount
        private void divideExpenseOwesYou()
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
                    //expenseShareUsers[i].paid_share = tbAmount.Text;

                }
                else
                {
                    //expenseShareUsers[i].paid_share = "0";
                    expenseShareUsers[i].owed_share = tbAmount.Text;
                }
            }
        }

        private bool divideExpenseUnequally()
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
                            //expenseShareUsers[i].paid_share = expenseAmount.ToString();
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
                            //expenseShareUsers[i].paid_share = expenseAmount.ToString();
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

        private void showUnequalSectionIfNeeded()
        {
            if (amountSplit != AmountSplit.UnequalSplit) ;
            //spUnequally.Visibility = System.Windows.Visibility.Collapsed;

            else
            {
                expense.cost = tbAmount.Text;
                divideExpenseEqually();
                //spUnequally.Visibility = System.Windows.Visibility.Visible;
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
