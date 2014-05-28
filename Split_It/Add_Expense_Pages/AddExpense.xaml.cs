using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections;
using Split_It_.Model;
using System.Windows.Media;
using Split_It_.Utils;
using System.ComponentModel;
using Split_It_.Controller;
using System.Collections.ObjectModel;

namespace Split_It_.Add_Expense_Pages
{
    public partial class AddExpense : PhoneApplicationPage
    {
        Expense expenseToAdd;
        ApplicationBarIconButton btnOkay;
        bool isAmountSet = false;
        BackgroundWorker addExpenseBackgroundWorker;
        BackgroundWorker getSupportedCurrenciesBackgroundWorker;
        ObservableCollection<Currency> currenciesList = new ObservableCollection<Currency>();
        
        public AddExpense()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] != null)
                expenseToAdd = PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] as Expense;

            addExpenseBackgroundWorker = new BackgroundWorker();
            addExpenseBackgroundWorker.WorkerSupportsCancellation = true;
            addExpenseBackgroundWorker.DoWork += new DoWorkEventHandler(addExpenseBackgroundWorker_DoWork);

            getSupportedCurrenciesBackgroundWorker = new BackgroundWorker();
            getSupportedCurrenciesBackgroundWorker.WorkerSupportsCancellation = true;
            getSupportedCurrenciesBackgroundWorker.DoWork += new DoWorkEventHandler(getSupportedCurrenciesBackgroundWorker_DoWork);

            if (!getSupportedCurrenciesBackgroundWorker.IsBusy)
                getSupportedCurrenciesBackgroundWorker.RunWorkerAsync();

            (App.Current.Resources["PhoneRadioCheckBoxCheckBrush"] as SolidColorBrush).Color = Colors.Black;
            createAppBar();
            
            this.friendListPicker.ItemsSource = App.friendsList;
            this.friendListPicker.SummaryForSelectedItemsDelegate = this.FriendSummaryDelegate;

            this.groupListPicker.ItemsSource = App.groupsList;
            this.groupListPicker.SummaryForSelectedItemsDelegate = this.GroupSummaryDelegate;
            this.groupListPicker.SelectedItem = getFromGroup();

            this.currencyListPicker.ItemsSource = this.currenciesList;
            this.currencyListPicker.SummaryForSelectedItemsDelegate = this.CurrencySummaryDelegate;

            if (expenseToAdd == null)
                expenseToAdd = new Expense();

            expenseDate.Value = DateTime.Now;
        }

        private Group getFromGroup()
        {
            if (expenseToAdd == null)
                return null;
            else
            {
                foreach (var group in App.groupsList)
                {
                    if (expenseToAdd.group_id == group.id)
                        return group;
                }
            }

            return null;
        }

        //returns true only if one or less group is selected
        private bool validGroupSelected()
        {
            if (this.groupListPicker.SelectedItems.Count > 1)
                return false;
            else
                return true;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //after you get a successful response from the server, make sure to set PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] as null
            
            //to hide the keyboard if any
            this.Focus();
            if (addExpenseBackgroundWorker.IsBusy != true)
            {
                busyIndicator.Content = "adding expense";
                busyIndicator.IsRunning = true;
                if (!isAmountSet)
                    divideExpenseEqually();

                expenseToAdd.currency_code = (currencyListPicker.SelectedItem as Currency).currency_code;
                expenseToAdd.payment = false;
                DateTime dateTime = expenseDate.Value ?? DateTime.Now;
                expenseToAdd.date = dateTime.ToString("yyyy-MM-ddTHH:mm:ssK");

                addExpenseBackgroundWorker.RunWorkerAsync();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] = null;
            NavigationService.GoBack();
        }

        private void createAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;
            ApplicationBar.BackgroundColor = (Color)Application.Current.Resources["green"];
            ApplicationBar.ForegroundColor = Colors.White;

            btnOkay = new ApplicationBarIconButton();
            btnOkay.IconUri = new Uri("/Assets/Icons/ok.png", UriKind.Relative);
            btnOkay.Text = "ok";
            btnOkay.IsEnabled = false;
            ApplicationBar.Buttons.Add(btnOkay);
            btnOkay.Click += new EventHandler(btnOk_Click);

            ApplicationBarIconButton btnCancel = new ApplicationBarIconButton();
            btnCancel.IconUri = new Uri("/Assets/Icons/cancel.png", UriKind.Relative);
            btnCancel.Text = "cancel";
            ApplicationBar.Buttons.Add(btnCancel);
            btnCancel.Click += new EventHandler(btnCancel_Click);
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
                summary = "no groups selected";
            }
            return summary;
        }

        private void groupListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.groupListPicker.SelectedItem == null)
                return;

            if (validGroupSelected())
            {
                Group selectedGroup = this.groupListPicker.SelectedItem as Group;
                expenseToAdd.group_id = selectedGroup.id;
                foreach (var member in selectedGroup.members)
                {
                    //you don't need to add yourself as you will be added by default.
                    if (member.id == App.currentUser.id)
                        continue;
                    this.friendListPicker.SelectedItems.Add(member);
                }
            }

            else
            {
                MessageBox.Show("Sorry you can only select one group", "Error", MessageBoxButton.OK);
                this.friendListPicker.SelectedItems.Clear();
                this.groupListPicker.SelectedItems.Clear();
                expenseToAdd.group_id = 0;
            }
        }

        private void friendListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            expenseToAdd.users = new List<Expense_Share>();
            //add yourself to the the list of expense users.
            expenseToAdd.users.Add(new Expense_Share() { user = App.currentUser, user_id = App.currentUser.id });
            enableOkButton();

            if (this.friendListPicker.SelectedItems == null)
                return;
            foreach (var item in this.friendListPicker.SelectedItems)
            {
                User user = item as User;
                expenseToAdd.users.Add(new Expense_Share() { user = user, user_id = user.id });
            }
        }

        private void divideExpenseEqually()
        {
            isAmountSet = true;
            int numberOfExpenseMembers = expenseToAdd.users.Count;
            double amountToSplit = Convert.ToDouble(expenseToAdd.cost);
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
                if (expenseToAdd.users[i].user_id == App.currentUser.id)
                {
                    expenseToAdd.users[i].paid_share = amountToSplit.ToString();
                    expenseToAdd.users[i].owed_share = currentUsersShare.ToString();

                }
                else
                {
                    expenseToAdd.users[i].paid_share = "0";
                    expenseToAdd.users[i].owed_share = perPersonShare.ToString();
                }
            }
        }

        private void enableOkButton()
        {
            if (this.friendListPicker.SelectedItems != null && !String.IsNullOrEmpty(tbAmount.Text) && !String.IsNullOrEmpty(tbDescription.Text))
            {
                btnOkay.IsEnabled = true;
            }
            else
                btnOkay.IsEnabled = false;
        }

        private void tbDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            expenseToAdd.description = tbDescription.Text;
            enableOkButton();
        }

        private void tbAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            expenseToAdd.cost = tbAmount.Text;
            enableOkButton();
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.friendListPicker.SelectedItems != null && !String.IsNullOrEmpty(tbAmount.Text) && !String.IsNullOrEmpty(tbDescription.Text))
            {
                divideExpenseEqually();
                PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] = expenseToAdd;

                //pass the expense to other page where user has more option to split the bill
            }

            else
            {
                MessageBox.Show("Please enter all the necessary details", "Error", MessageBoxButton.OK);
            }
        }

        private void addExpenseBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ModifyDatabase modify = new ModifyDatabase(_addExpenseCompleted);
            modify.addExpense(expenseToAdd);
        }

        private void _addExpenseCompleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] = null;
                    busyIndicator.IsRunning = false;
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    busyIndicator.IsRunning = false;

                    if (errorCode == HttpStatusCode.Unauthorized)
                    {
                        Util.logout();
                        NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show("Unable to add expense", "Error", MessageBoxButton.OK);
                    }
                });
            }
        }

        private void getSupportedCurrenciesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Currency defaultCurrency = null;
            QueryDatabase query = new QueryDatabase();
            foreach (var item in query.getSupportedCurrencies())
            {
                if (item.currency_code == App.currentUser.default_currency && String.IsNullOrEmpty(expenseToAdd.currency_code))
                    defaultCurrency = item;

                else if (item.currency_code == expenseToAdd.currency_code)
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
    }
}