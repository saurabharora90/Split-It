using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Split_It_.Controller;
using Split_It_.Utils;
using Split_It_.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Data;

namespace Split_It_
{
    public partial class MainPage : PhoneApplicationPage
    {
        ObservableCollection<User> balanceFriends = new ObservableCollection<User>();
        ObservableCollection<User> youOweFriends = new ObservableCollection<User>();
        ObservableCollection<User> owesYouFriends = new ObservableCollection<User>();
        ObservableCollection<User> friendsList = new ObservableCollection<User>();

        ObservableCollection<Group> groupsList = new ObservableCollection<Group>();
        ObservableCollection<Expense> expensesList = new ObservableCollection<Expense>();

        //Use a BackgroundWorker to load data from database (except for friends) as expenses
        //and groups are time consuming operations
        BackgroundWorker dataLoadingBackgroundWorker;
        BackgroundWorker syncDatabaseBackgroundWorker;

        SyncDatabase databaseSync;
        private object o = new object();
        private int pageNo = 0;
        private bool morePages = true;

        private ApplicationBar dashBoardAppBar, expenseAppBar;
        private ApplicationBarMenuItem btnAllFriends, btnBalanceFriends, btnYouOweFriends, btnOwesYouFriends;
        private double postiveBalance = 0, negativeBalance = 0, totalBalance = 0;
        private NetBalances netBalanceObj = new NetBalances();

        public MainPage()
        {
            InitializeComponent();

            App.accessToken = Util.getAccessToken();
            App.accessTokenSecret = Util.getAccessTokenSecret();

            llsFriends.ItemsSource = balanceFriends;
            llsExpenses.ItemsSource = expensesList;
            llsGroups.ItemsSource = groupsList;

            dataLoadingBackgroundWorker = new BackgroundWorker();
            dataLoadingBackgroundWorker.WorkerSupportsCancellation = true;
            dataLoadingBackgroundWorker.DoWork += new DoWorkEventHandler(dataLoadingBackgroundWorker_DoWork);

            syncDatabaseBackgroundWorker = new BackgroundWorker();
            syncDatabaseBackgroundWorker.WorkerSupportsCancellation = true;
            syncDatabaseBackgroundWorker.DoWork += new DoWorkEventHandler(syncDatabaseBackgroundWorker_DoWork);

            setupAppBars();
            populateData();
        }

        private void setupAppBars()
        {
            dashBoardAppBar = new ApplicationBar();
            dashBoardAppBar.Mode = ApplicationBarMode.Minimized;
            dashBoardAppBar.Opacity = 1.0;
            dashBoardAppBar.IsMenuEnabled = true;

            btnAllFriends = new ApplicationBarMenuItem();
            btnAllFriends.Text = "all";
            dashBoardAppBar.MenuItems.Add(btnAllFriends);
            btnAllFriends.Click += new EventHandler(btnAllFriends_Click);

            btnBalanceFriends = new ApplicationBarMenuItem();
            btnBalanceFriends.Text = "balance";
            dashBoardAppBar.MenuItems.Add(btnBalanceFriends);
            btnBalanceFriends.Click += new EventHandler(btnBalanceFriends_Click);

            btnYouOweFriends = new ApplicationBarMenuItem();
            btnYouOweFriends.Text = "you owe";
            dashBoardAppBar.MenuItems.Add(btnYouOweFriends);
            btnYouOweFriends.Click += new EventHandler(btnYouOweFriends_Click);

            btnOwesYouFriends = new ApplicationBarMenuItem();
            btnOwesYouFriends.Text = "owes you";
            dashBoardAppBar.MenuItems.Add(btnOwesYouFriends);
            btnOwesYouFriends.Click += new EventHandler(btnOwesYouFriends_Click);

            expenseAppBar = new ApplicationBar();
            expenseAppBar.Mode = ApplicationBarMode.Minimized;
            expenseAppBar.Opacity = 1.0;
            expenseAppBar.IsVisible = true;
            expenseAppBar.IsMenuEnabled = false;
            expenseAppBar.BackgroundColor = (Color)Application.Current.Resources["green"];
            expenseAppBar.ForegroundColor = Colors.White;

            //add expense button
            ApplicationBarIconButton btnAddExpense = new ApplicationBarIconButton();
            btnAddExpense.IconUri = new Uri("/Assets/Icons/add.png", UriKind.Relative);
            btnAddExpense.Text = "add";
            expenseAppBar.Buttons.Add(btnAddExpense);
            btnAddExpense.Click += new EventHandler(btnAddExpense_Click);
        }

        private void btnAddExpense_Click(object sender, EventArgs e)
        {
            llsFriends.ItemsSource = friendsList;
        }
        
        private void btnAllFriends_Click(object sender, EventArgs e)
        {
            llsFriends.ItemsSource = friendsList;
        }

        private void btnYouOweFriends_Click(object sender, EventArgs e)
        {
            llsFriends.ItemsSource = youOweFriends;
        }

        private void btnOwesYouFriends_Click(object sender, EventArgs e)
        {
            llsFriends.ItemsSource = owesYouFriends;
        }

        private void btnBalanceFriends_Click(object sender, EventArgs e)
        {
            llsFriends.ItemsSource = balanceFriends;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                //populateData();
                return;
            }

            String firstUse;

            //This condition will only be true if the user has launched this page. This paramter (afterLogin) wont be there
            //if the page has been accessed from the back stack
            if (NavigationContext.QueryString.TryGetValue("afterLogin", out firstUse))
            {
                if (firstUse.Equals("true"))
                {
                    //do not allow him to go back to the Login Page. therefore clear the back stack
                    while (NavigationService.CanGoBack) NavigationService.RemoveBackEntry();
                    databaseSync = new SyncDatabase(_SyncConpleted, true);
                }
                else
                    databaseSync = new SyncDatabase(_SyncConpleted, false);

                if (syncDatabaseBackgroundWorker.IsBusy != true)
                {
                    SystemTray.ProgressIndicator = new ProgressIndicator();
                    SystemTray.ProgressIndicator.IsIndeterminate = true;
                    SystemTray.ProgressIndicator.IsVisible = true;
                    if (firstUse.Equals("true"))
                        SystemTray.ProgressIndicator.Text = "Setting up for first use";
                    else
                        SystemTray.ProgressIndicator.Text = "Syncing";

                    syncDatabaseBackgroundWorker.RunWorkerAsync();
                }

            }
        }

        private void dataLoadingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadExpensesAndGroups();
        }

        private void syncDatabaseBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            databaseSync.performSync();
        }

        private void loadFriends()
        {
            friendsList.Clear();
            youOweFriends.Clear();
            owesYouFriends.Clear();
            balanceFriends.Clear();
            postiveBalance = 0;
            negativeBalance = 0;
            totalBalance = 0;

            QueryDatabase obj = new QueryDatabase();
            foreach (var friend in obj.getAllFriends())
            {
                friendsList.Add(friend);
                double balance = Util.getBalance(friend.balance);
                if (balance > 0)
                {
                    postiveBalance += balance;
                    totalBalance += balance;
                    owesYouFriends.Add(friend);
                    balanceFriends.Add(friend);
                }

                if (balance < 0)
                {
                    negativeBalance += balance;
                    totalBalance += balance;
                    youOweFriends.Add(friend);
                    balanceFriends.Add(friend);
                }
            }

            if (App.currentUser == null)
                return;

            netBalanceObj.setBalances(App.currentUser.default_currency, totalBalance, postiveBalance, negativeBalance);
            btnOwesYouFriends.Text = netBalanceObj.PositiveBalance;
            btnYouOweFriends.Text = netBalanceObj.NegativeBalance;
            btnBalanceFriends.Text = netBalanceObj.NetBalance;
            obj.closeDatabaseConnection();
        }

        private void populateData()
        {
            loadFriends();
            if (dataLoadingBackgroundWorker.IsBusy != true)
            {
                dataLoadingBackgroundWorker.RunWorkerAsync();
            }
        }

        private void loadExpensesAndGroups()
        {
            //the rest of the work is done in a backgroundworker
            QueryDatabase obj = new QueryDatabase();
            List<Expense> allExpenses = obj.getAllExpenses(pageNo);

            Dispatcher.BeginInvoke(() =>
            {
                if(pageNo == 0)
                    expensesList.Clear();

                if (allExpenses == null || allExpenses.Count == 0)
                    morePages = false;
                else
                    morePages = true;

                foreach (var expense in allExpenses)
                {
                    expensesList.Add(expense);
                }
            });

            obj.closeDatabaseConnection();
        }

        private void _SyncConpleted(bool success)
        {
            if (success)
            {
                Dispatcher.BeginInvoke(() =>
            {
                if (dataLoadingBackgroundWorker.WorkerSupportsCancellation == true)
                    dataLoadingBackgroundWorker.CancelAsync();
                //pageNo = 0;
                populateData();

                if(SystemTray.ProgressIndicator!=null)
                    SystemTray.ProgressIndicator.IsVisible = false;
            });
            }
        }

        private void llsExpenses_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            lock (o)
            {
                Expense expense = e.Container.Content as Expense;
                if (expense != null)
                {
                    int offset = 2;

                    if (dataLoadingBackgroundWorker.IsBusy != true && morePages && expensesList.Count - expensesList.IndexOf(expense) <= offset)
                    {
                        pageNo++;
                        dataLoadingBackgroundWorker.RunWorkerAsync();
                    }
                }
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    ApplicationBar = dashBoardAppBar;
                    ApplicationBar.IsVisible = true;
                    break;

                case 1:
                    ApplicationBar = expenseAppBar;
                    ApplicationBar.IsVisible = true;
                    break;
            }

            ApplicationBar.BackgroundColor = (Color)Application.Current.Resources["green"];
            ApplicationBar.ForegroundColor = Colors.White;
        }

        private void llsExpenses_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            Expense selectedExpense = selector.SelectedItem as Expense;

            if (selectedExpense == null)
                return;

            PhoneApplicationService.Current.State[Constants.SELECTED_EXPENSE] = selectedExpense;
            NavigationService.Navigate(new Uri("/ExpenseDetail.xaml", UriKind.Relative));
        }

        private void llsFriends_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            User selectedUser = selector.SelectedItem as User;

            if (selectedUser == null)
                return;

            PhoneApplicationService.Current.State[Constants.SELECTED_USER] = selectedUser;
            NavigationService.Navigate(new Uri("/UserDetails.xaml", UriKind.Relative));
        }
    }
}