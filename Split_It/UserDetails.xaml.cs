using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Split_It_.Model;
using Split_It_.Utils;
using System.ComponentModel;
using Split_It_.Controller;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Microsoft.Phone.Tasks;

namespace Split_It_
{
    public partial class UserDetails : PhoneApplicationPage
    {
        User selectedUser;
        BackgroundWorker userExpensesBackgroundWorker;
        ObservableCollection<Expense> expensesList = new ObservableCollection<Expense>();
        private int pageNo = 0;
        private bool morePages = true;
        private object o = new object();

        public UserDetails()
        {
            InitializeComponent();

            selectedUser = PhoneApplicationService.Current.State[Constants.SELECTED_USER] as User;

            llsExpenses.ItemsSource = expensesList;
            this.llsExpenses.DataRequested += this.OnDataRequested;
            
            userExpensesBackgroundWorker = new BackgroundWorker();
            userExpensesBackgroundWorker.WorkerSupportsCancellation = true;
            userExpensesBackgroundWorker.DoWork += new DoWorkEventHandler(userExpensesBackgroundWorker_DoWork);

            if (userExpensesBackgroundWorker.IsBusy != true)
            {
                userExpensesBackgroundWorker.RunWorkerAsync();
            }

            createAppBar();
            this.DataContext = selectedUser;
            if (selectedUser.balance.Count == 0)
                selectedUser.balance.Add(new Balance_User() { amount = "0", currency_code = App.currentUser.default_currency, user_id = selectedUser.id });
            this.balanceList.ItemsSource = selectedUser.balance;
        }

        private void createAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;
            ApplicationBar.BackgroundColor = (Color) Application.Current.Resources["green"];
            ApplicationBar.ForegroundColor = Colors.White;

            //Add expense to friend
            ApplicationBarIconButton btnAddExpense = new ApplicationBarIconButton();
            btnAddExpense.IconUri = new Uri("/Assets/Icons/add.png", UriKind.Relative);
            btnAddExpense.Text = "add";
            ApplicationBar.Buttons.Add(btnAddExpense);
            btnAddExpense.Click += new EventHandler(btnAddExpense_Click);

            ApplicationBarIconButton btnReminder = new ApplicationBarIconButton();
            btnReminder.IconUri = new Uri("/Assets/Icons/feature.email.png", UriKind.Relative);
            btnReminder.Text = "reminder";

            //Settle up button
            ApplicationBarIconButton btnSettle = new ApplicationBarIconButton();
            btnSettle.IconUri = new Uri("/Assets/Icons/settle.png", UriKind.Relative);
            btnSettle.Text = "settle";

            //disable both by default
            btnReminder.IsEnabled = false;
            btnSettle.IsEnabled = false;

            if (hasOwesYouBalance())
                btnReminder.IsEnabled = true;

            if (hasYouOweBalance() || hasOwesYouBalance())
                btnSettle.IsEnabled = true;

            ApplicationBar.Buttons.Add(btnReminder);
            ApplicationBar.Buttons.Add(btnSettle);
            btnReminder.Click += new EventHandler(btnReminder_Click);
            btnSettle.Click += new EventHandler(btnSettle_Click);
        }

        private bool hasYouOweBalance()
        {
            foreach (var balance in selectedUser.balance)
            {
                if (System.Convert.ToDouble(balance.amount, System.Globalization.CultureInfo.InvariantCulture) < 0)
                    return true;
            }

            return false;
        }

        private bool hasOwesYouBalance()
        {
            foreach (var balance in selectedUser.balance)
            {
                if (System.Convert.ToDouble(balance.amount, System.Globalization.CultureInfo.InvariantCulture) > 0)
                    return true;
            }

            return false;
        }

        private void btnAddExpense_Click(object sender, EventArgs e)
        {
            Expense expenseToAdd = new Expense();
            expenseToAdd.specificUserId = selectedUser.id;

            PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] = expenseToAdd;
            NavigationService.Navigate(new Uri("/Add_Expense_Pages/AddExpense.xaml", UriKind.Relative));
        }

        private void btnSettle_Click(object sender, EventArgs e)
        {
            int navParams;
            if (hasOwesYouBalance())
                navParams = Constants.PAYMENT_FROM;
            else
                navParams = Constants.PAYMENT_TO;

            PhoneApplicationService.Current.State[Constants.PAYMENT_USER] = selectedUser;
            PhoneApplicationService.Current.State[Constants.PAYMENT_TYPE] = navParams;
            PhoneApplicationService.Current.State[Constants.PAYMENT_GROUP] = 0;

            NavigationService.Navigate(new Uri("/Add_Expense_Pages/AddPayment.xaml", UriKind.Relative));
        }

        private void btnReminder_Click(object sender, EventArgs e)
        {
            string appUrl = "";
            string reminderText = "Hey there,\n\nThis is just a note to settle up on Splitwise as soon as you get the chance.\n\n";
            string thanks = "Thanks,\n";
            string userName = App.currentUser.first_name;
            string sentVia = "\n\nSent via,\n";
            string appName = "Split it! A splitwise client for windows phone\n\n";
            string downloadApp = "Download app at: " + appUrl;

            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "Settle up on Splitwise";
            emailComposeTask.Body = reminderText + thanks + userName + sentVia + appName + downloadApp;
            emailComposeTask.To = selectedUser.email;

            emailComposeTask.Show();
        }

        private void userExpensesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadExpenses();
        }

        private void loadExpenses()
        {
            //the rest of the work is done in a backgroundworker
            QueryDatabase obj = new QueryDatabase();
            List<Expense> allExpenses = obj.getExpensesForUser(selectedUser.id, pageNo);

            Dispatcher.BeginInvoke(() =>
            {
                if (allExpenses == null || allExpenses.Count == 0)
                    morePages = false;

                foreach (var expense in allExpenses)
                {
                    expensesList.Add(expense);
                }
            });

            obj.closeDatabaseConnection();
        }

        private void OnDataRequested(object sender, EventArgs e)
        {
            lock (o)
            {
                if (userExpensesBackgroundWorker.IsBusy != true && morePages)
                {
                    pageNo++;
                    userExpensesBackgroundWorker.RunWorkerAsync(false);
                }
            }
        }

        private void llsExpenses_Tap(object sender, SelectionChangedEventArgs e)
        {
            if (llsExpenses.SelectedItem == null)
                return;
            Expense selectedExpense = llsExpenses.SelectedItem as Expense;

            PhoneApplicationService.Current.State[Constants.SELECTED_EXPENSE] = selectedExpense;
            NavigationService.Navigate(new Uri("/ExpenseDetail.xaml", UriKind.Relative));

            llsExpenses.SelectedItem = null;
        }
    }
}