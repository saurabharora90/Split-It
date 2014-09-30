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
using System.Windows.Controls.Primitives;
using Telerik.Windows.Controls;

namespace Split_It_.UserControls
{
    public partial class ExpenseUserControl : UserControl
    {
        public Expense expense;

        private BackgroundWorker getSupportedCurrenciesBackgroundWorker;
        
        public ObservableCollection<Expense_Share> friends = new ObservableCollection<Expense_Share>();
        public ObservableCollection<Group> groupsList = new ObservableCollection<Group>();

        private ObservableCollection<Currency> currenciesList = new ObservableCollection<Currency>();
        private ObservableCollection<Expense_Share> expenseShareUsers = new ObservableCollection<Expense_Share>();

        //this will ONLY be null if there are multiple paid by users.
        private Expense_Share PaidByUser;
        
        //By default the amount is split equally
        public AmountSplit amountSplit = AmountSplit.EqualSplit;

        string decimalsep = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;

        //Popups
        Telerik.Windows.Controls.RadWindow PayeeWindow;
        Telerik.Windows.Controls.RadWindow SplitUnequallyWindow;
        Telerik.Windows.Controls.RadWindow MultiplePayeeWindow;
        Action<bool> DimBackground;

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
            this.SplitTypeListPicker.SelectionChanged += SplitTypeListPicker_SelectionChanged;

            this.expenseTypeListPicker.SelectionChanged += expenseTypeListPicker_SelectionChanged;

            if (expense == null)
            {
                expense = new Expense();
                expense.group_id = 0; //by default the expense is in no group
            }

            //add current user
            expenseShareUsers.Add(getCurrentUser());
            PaidByUser = getCurrentUser();
            tbPaidBy.Text = PaidByUser.ToString();
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

        private void tbPaidBy_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (expenseShareUsers.Count <= 1)
                return;

            //need can proceed as user can select multiple payee and we need cost for that.
            if (canProceed())
            {
                PayeeWindow = new Telerik.Windows.Controls.RadWindow();
                SelectPayeePopUpControl ChoosePayeePopup = new SelectPayeePopUpControl(expenseShareUsers, _PayeeClose);
                ChoosePayeePopup.MaxHeight = App.Current.Host.Content.ActualHeight / 1.1;
                ChoosePayeePopup.MaxWidth = App.Current.Host.Content.ActualWidth / 1.1;

                PayeeWindow.Content = ChoosePayeePopup;
                ShowRadWindow(ref PayeeWindow);
            }
        }

        /**
         * If isMultiplePayer is true then SelectedUser can be null
         */
        private void _PayeeClose(Expense_Share SelectedUser, bool isMultiplePayer) 
        {
            PayeeWindow.IsOpen = false;

            //Set the paid amount of everyone to 0 and the selected user's hasPaid to true.
            //The selected user's cost is set during the setupExpense method.
            if (isMultiplePayer)
            {
                //Show the multiple payer popup
                showMultiplePayeePopUp();
            }
            else
            {
                for (int i = 0; i < expenseShareUsers.Count; i++)
                {
                    expenseShareUsers[i].paid_share = "0.0";

                    if (expenseShareUsers[i].Equals(SelectedUser))
                        expenseShareUsers[i].hasPaid = true;
                    else
                        expenseShareUsers[i].hasPaid = false;
                }
                PaidByUser = SelectedUser;
                tbPaidBy.Text = PaidByUser.ToString(); ;
            }
        }

        private void showMultiplePayeePopUp()
        {
            MultiplePayeeWindow = new RadWindow();
            MultiplePayeeInputPopUpControl MultiplePayeeInputPopup = new MultiplePayeeInputPopUpControl
                                                            (ref expenseShareUsers, _MultiplePayeeInputClose, Convert.ToDouble(tbAmount.Text));
            MultiplePayeeInputPopup.MaxHeight = App.Current.Host.Content.ActualHeight / 1.1;
            MultiplePayeeInputPopup.MaxWidth = App.Current.Host.Content.ActualWidth / 1.1;

            MultiplePayeeWindow.Content = MultiplePayeeInputPopup;
            ShowRadWindow(ref MultiplePayeeWindow);
        }

        //This is called after validation has been done and okay has been pressed.
        private void _MultiplePayeeInputClose()
        {
            MultiplePayeeWindow.IsOpen = false;
            PaidByUser = null;
            tbPaidBy.Text = "Multiple users";
        }

        void SplitTypeListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            amountSplit = (AmountSplit)this.SplitTypeListPicker.SelectedItem;
            if (amountSplit.id == AmountSplit.TYPE_SPLIT_UNEQUALLY)
            {
                if (canProceed())
                {
                    //show unequall split pop up;
                    SplitUnequallyWindow = new RadWindow();
                    UnequallySplit UnequallySplitPopup = new UnequallySplit(expenseShareUsers, Convert.ToDouble(tbAmount.Text), _UnequallyClose);
                    UnequallySplitPopup.MaxHeight = App.Current.Host.Content.ActualHeight / 1.1;
                    UnequallySplitPopup.MaxWidth = App.Current.Host.Content.ActualWidth / 1.1;

                    SplitUnequallyWindow.Content = UnequallySplitPopup;
                    ShowRadWindow(ref SplitUnequallyWindow);
                }
                else
                    SplitTypeListPicker.SelectedItem = AmountSplit.EqualSplit;
            }
        }

        private void _UnequallyClose(ObservableCollection<Expense_Share> users)
        {
            SplitUnequallyWindow.IsOpen = false;
            this.expenseShareUsers = users;
        }
        
        private void ShowRadWindow(ref RadWindow window)
        {
            window.Placement = Telerik.Windows.Controls.PlacementMode.CenterCenter;
            window.IsOpen = true;
            window.WindowClosed += RadWindow_WindowClosed;
            DimBackground(true);
        }

        void RadWindow_WindowClosed(object sender, WindowClosedEventArgs e)
        {
            DimBackground(false);
        }

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

            foreach (var item in e.AddedItems)
            {
                if(expenseShareUsers.Contains(item))
                    return;
                expenseShareUsers.Add(item as Expense_Share);
            }

            foreach (var item in e.RemovedItems)
            {
                expenseShareUsers.Remove(item as Expense_Share);
                if (item == PaidByUser)
                {
                    PaidByUser = getCurrentUser();
                    tbPaidBy.Text = PaidByUser.ToString();
                }
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

        private bool canProceed()
        {
            if (String.IsNullOrEmpty(tbAmount.Text) || friendListPicker.SelectedItems == null || friendListPicker.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select expense friends and enter the expense amount.", "Error", MessageBoxButton.OK);
                return false;
            }

            return true;
        }
        
#endregion
        
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

                ExpenseType type = (ExpenseType)this.expenseTypeListPicker.SelectedItem;
                //Type is null when expenseTypeListPicker is not visibile, i.e. more than 1 expense user is selected.
                if (type == null)
                    proceed = SplitBillType();
                else
                {
                    switch (type.id)
                    {
                        case ExpenseType.TYPE_FRIEND_OWES:
                            PaidByUser = getCurrentUser();
                            fullExpenseSplit();
                            break;
                        case ExpenseType.TYPE_YOU_OWE:
                            PaidByUser = friendListPicker.SelectedItem as Expense_Share;
                            fullExpenseSplit();
                            break;
                        case ExpenseType.TYPE_SPLIT_BILL:
                            proceed = SplitBillType();
                            break;
                        default:
                            break;
                    }
                }

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

        private bool SplitBillType()
        {
            bool proceed = true;
            //This can have split equally or unequally;
            switch (amountSplit.id)
            {
                case AmountSplit.TYPE_SPLIT_EQUALLY:
                    proceed = divideExpenseEqually();
                    break;
                case AmountSplit.TYPE_SPLIT_UNEQUALLY:
                    proceed = divideExpenseUnequally();
                    break;
                default:
                    break;
            }

            return proceed;
        }

        private bool divideExpenseEqually()
        {
            double totalPaidBy = 0;

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
                if (PaidByUser!=null && expenseShareUsers[i].user_id == PaidByUser.user.id)
                {
                    expenseShareUsers[i].paid_share = amountToSplit.ToString();
                }

                if(expenseShareUsers[i].user_id == App.currentUser.id)
                    expenseShareUsers[i].owed_share = currentUsersShare.ToString();
                else
                {
                    //do not set paid_share as 0 as it might override the value set by multiple payers.
                    //expenseShareUsers[i].paid_share = "0";
                    expenseShareUsers[i].owed_share = perPersonShare.ToString();
                }
                if (!String.IsNullOrEmpty(expenseShareUsers[i].paid_share))
                    totalPaidBy += Convert.ToDouble(expenseShareUsers[i].paid_share);

                expenseShareUsers[i].share = 1;
            }

            if (totalPaidBy == amountToSplit)
                return true;
            else
            {
                MessageBox.Show("The \"Paid by\" does not add upto the total expense cost.", "Error", MessageBoxButton.OK);
                return false;
            }
        }

        //The unequallySplit popup has already taken care of owed_share. here we only need to handle the paidshare
        //the paid share is handled with the help of PaidByUser. If this is null, then it means we have a Multiple Payee scenario
        //and in that case, even the paid share is already handled for us by the MultiplePayeeInputPopUpControl
        private bool divideExpenseUnequally()
        {
            double totalPaidBy = 0;
            double totalOwed = 0;
            double amountToSplit = Convert.ToDouble(expense.cost);
            int numberOfExpenseMembers = expenseShareUsers.Count;

            for (int i = 0; i < numberOfExpenseMembers; i++)
            {
                if (PaidByUser != null && expenseShareUsers[i].user_id == PaidByUser.user.id)
                {
                    expenseShareUsers[i].paid_share = amountToSplit.ToString();
                }

                if (!String.IsNullOrEmpty(expenseShareUsers[i].paid_share))
                    totalPaidBy += Convert.ToDouble(expenseShareUsers[i].paid_share);

                if (!String.IsNullOrEmpty(expenseShareUsers[i].owed_share))
                    totalOwed += Convert.ToDouble(expenseShareUsers[i].owed_share);
            }

            if (totalPaidBy == amountToSplit && totalOwed == amountToSplit)
                return true;
            else
            {
                if(totalPaidBy != amountToSplit)
                    MessageBox.Show("The \"Paid by\" does not add upto the total expense cost.", "Error", MessageBoxButton.OK);

                if (totalOwed != amountToSplit)
                    MessageBox.Show("The \"Unequal Split\" does not add upto the total expense cost.", "Error", MessageBoxButton.OK);

                return false;
            }
        }

        private void fullExpenseSplit()
        {
            //only you and one more user should be there to access this feature
            if (expenseShareUsers.Count != 2)
                throw new IndexOutOfRangeException();
            int numberOfExpenseMembers = expenseShareUsers.Count;
            for (int i = 0; i < numberOfExpenseMembers; i++)
            {
                if (expenseShareUsers[i].user_id == PaidByUser.user.id)
                {
                    expenseShareUsers[i].paid_share = expense.cost;
                    expenseShareUsers[i].owed_share = "0";
                }
                else
                {
                    expenseShareUsers[i].paid_share = "0";
                    expenseShareUsers[i].owed_share = expense.cost;
                }
            }
        }

        public void setDimBackGround(Action<bool> dim)
        {
            this.DimBackground = dim;
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
