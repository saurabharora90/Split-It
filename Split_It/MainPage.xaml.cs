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
using Microsoft.Phone.Tasks;
using System.Globalization;
using Windows.ApplicationModel.Store;

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
        BackgroundWorker expenseLoadingBackgroundWorker;
        BackgroundWorker groupLoadingBackgroundWorker;

        BackgroundWorker syncDatabaseBackgroundWorker;
        SyncDatabase databaseSync;
       
        private object o = new object();
        private int pageNo = 0;
        private bool morePages = true, hasDataLoaded = false;

        private ApplicationBarMenuItem btnAllFriends, btnBalanceFriends, btnYouOweFriends, btnOwesYouFriends;
        private double postiveBalance = 0, negativeBalance = 0, totalBalance = 0;
        private NetBalances netBalanceObj = new NetBalances();

        public MainPage()
        {
            InitializeComponent();
            setupAppBars();
            resetAppPromo();

            llsFriends.ItemsSource = balanceFriends;
            llsExpenses.ItemsSource = expensesList;
            llsGroups.ItemsSource = groupsList;

            this.llsExpenses.DataRequested += this.OnDataRequested;

            expenseLoadingBackgroundWorker = new BackgroundWorker();
            expenseLoadingBackgroundWorker.WorkerSupportsCancellation = true;
            expenseLoadingBackgroundWorker.DoWork += new DoWorkEventHandler(expenseLoadingBackgroundWorker_DoWork);

            groupLoadingBackgroundWorker = new BackgroundWorker();
            groupLoadingBackgroundWorker.WorkerSupportsCancellation = true;
            groupLoadingBackgroundWorker.DoWork += new DoWorkEventHandler(groupLoadingBackgroundWorker_DoWork);
            
            populateData();

            syncDatabaseBackgroundWorker = new BackgroundWorker();
            syncDatabaseBackgroundWorker.WorkerSupportsCancellation = true;
            syncDatabaseBackgroundWorker.DoWork += new DoWorkEventHandler(syncDatabaseBackgroundWorker_DoWork);

            if (App.AdsRemoved)
                beer.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void setupAppBars()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.BackgroundColor = (Color)Application.Current.Resources["green"];
            ApplicationBar.ForegroundColor = Colors.White;

            btnAllFriends = new ApplicationBarMenuItem();
            btnAllFriends.Text = "all";
            btnAllFriends.Click += new EventHandler(btnAllFriends_Click);

            btnBalanceFriends = new ApplicationBarMenuItem();
            btnBalanceFriends.Text = "balance";
            btnBalanceFriends.Click += new EventHandler(btnBalanceFriends_Click);

            btnYouOweFriends = new ApplicationBarMenuItem();
            btnYouOweFriends.Text = "you owe";
            btnYouOweFriends.Click += new EventHandler(btnYouOweFriends_Click);

            btnOwesYouFriends = new ApplicationBarMenuItem();
            btnOwesYouFriends.Text = "owes you";
            btnOwesYouFriends.Click += new EventHandler(btnOwesYouFriends_Click);

            //add expense button
            ApplicationBarIconButton btnAddExpense = new ApplicationBarIconButton();
            btnAddExpense.IconUri = new Uri("/Assets/Icons/add.png", UriKind.Relative);
            btnAddExpense.Text = "add";
            ApplicationBar.Buttons.Add(btnAddExpense);
            btnAddExpense.Click += new EventHandler(btnAddExpense_Click);

            //search expense button
            ApplicationBarIconButton btnSearchExpense = new ApplicationBarIconButton();
            btnSearchExpense.IconUri = new Uri("/Assets/Icons/feature.search.png", UriKind.Relative);
            btnSearchExpense.Text = "search";
            ApplicationBar.Buttons.Add(btnSearchExpense);
            btnSearchExpense.Click += new EventHandler(btnSearchExpense_Click);

            ApplicationBar.MenuItems.Add(btnAllFriends);
            ApplicationBar.MenuItems.Add(btnBalanceFriends);
            ApplicationBar.MenuItems.Add(btnYouOweFriends);
            ApplicationBar.MenuItems.Add(btnOwesYouFriends);
        }

        private void btnAddExpense_Click(object sender, EventArgs e)
        {
            PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] = null;
            NavigationService.Navigate(new Uri("/Add_Expense_Pages/AddExpense.xaml", UriKind.Relative));
        }

        private void btnSearchExpense_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ExpenseSearch.xaml", UriKind.Relative));
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
            more.DataContext = App.currentUser;
            while (NavigationService.CanGoBack) NavigationService.RemoveBackEntry();

            if (e.NavigationMode == NavigationMode.Back)
            {
                //populateData();
                return;
            }
            else
            {
                String firstUse;
                databaseSync = new SyncDatabase(_SyncConpleted);
                busyIndicator.Content = "Syncing";
                //This condition will only be true if the user has launched this page. This paramter (afterLogin) wont be there
                //if the page has been accessed from the back stack
                if (NavigationContext.QueryString.TryGetValue("afterLogin", out firstUse))
                {
                    if (firstUse.Equals("true"))
                    {
                        databaseSync.isFirstSync(true);
                        busyIndicator.Content = "Setting up for first use";
                    }
                }

                if (syncDatabaseBackgroundWorker.IsBusy != true)
                {
                    busyIndicator.IsRunning = true;
                    syncDatabaseBackgroundWorker.RunWorkerAsync();
                }
            }
        }

        private void expenseLoadingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadExpenses();
        }

        private void groupLoadingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadGroups();
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

            //only show balance below in the user's default currency
            foreach (var friend in obj.getAllFriends())
            {
                friendsList.Add(friend);
                Balance_User defaultBalance = Util.getDefaultBalance(friend.balance);
                double balance = System.Convert.ToDouble(defaultBalance.amount, CultureInfo.InvariantCulture);
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
            {
                obj.closeDatabaseConnection();
                return;
            }

            //if default currency is not set then dont display the balances. Only the text is enough.
            if (App.currentUser.default_currency == null)
                return;
            netBalanceObj.setBalances(App.currentUser.default_currency, totalBalance, postiveBalance, negativeBalance);
            btnOwesYouFriends.Text = netBalanceObj.PositiveBalance;
            btnYouOweFriends.Text = netBalanceObj.NegativeBalance;
            btnBalanceFriends.Text = netBalanceObj.NetBalance;
            obj.closeDatabaseConnection();

            more.DataContext = App.currentUser;
        }

        private void populateData()
        {
            loadFriends();
            if (expenseLoadingBackgroundWorker.IsBusy != true)
            {
                expenseLoadingBackgroundWorker.RunWorkerAsync();
            }

            if (groupLoadingBackgroundWorker.IsBusy != true)
            {
                groupLoadingBackgroundWorker.RunWorkerAsync();
            }
        }

        private void loadExpenses()
        {
            lock (o)
            {
                QueryDatabase obj = new QueryDatabase();
                List<Expense> allExpenses = obj.getAllExpenses(pageNo);

                Dispatcher.BeginInvoke(() =>
                {
                    if (pageNo == 0)
                        expensesList.Clear();

                    if (allExpenses == null || allExpenses.Count == 0)
                        morePages = false;
                    else
                        morePages = true;

                    if (allExpenses != null)
                    {
                        foreach (var expense in allExpenses)
                        {
                            expensesList.Add(expense);
                        }
                    }
                });

                obj.closeDatabaseConnection();
            }
        }

        private void loadGroups()
        {
            QueryDatabase obj = new QueryDatabase();
            List<Group> allGroups = obj.getAllGroups();
            Dispatcher.BeginInvoke(() =>
            {
                groupsList.Clear();
                if (allGroups != null)
                {
                    foreach (var group in allGroups)
                    {
                        groupsList.Add(group);
                    }
                }
            });
            obj.closeDatabaseConnection();
        }

        private void OnDataRequested(object sender, EventArgs e)
        {
            lock (o)
            {
                if (expenseLoadingBackgroundWorker.IsBusy != true && morePages && hasDataLoaded)
                {
                    pageNo++;
                    expenseLoadingBackgroundWorker.RunWorkerAsync(false);
                }
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    ApplicationBar.IsMenuEnabled = true;
                    break;

                default:
                    ApplicationBar.IsMenuEnabled = false;
                    break;
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

        private void llsFriends_Tap(object sender, SelectionChangedEventArgs e)
        {
            if (llsFriends.SelectedItem == null)
                return;
            User selectedUser = llsFriends.SelectedItem as User;

            PhoneApplicationService.Current.State[Constants.SELECTED_USER] = selectedUser;
            NavigationService.Navigate(new Uri("/UserDetails.xaml", UriKind.Relative));

            llsFriends.SelectedItem = null;
        }

        private void llsGroups_Tap(object sender, SelectionChangedEventArgs e)
        {
            if (llsGroups.SelectedItem == null)
                return;
            Group selectedGroup = llsGroups.SelectedItem as Group;

            PhoneApplicationService.Current.State[Constants.SELECTED_GROUP] = selectedGroup;
            NavigationService.Navigate(new Uri("/GroupDetail.xaml", UriKind.Relative));

            llsGroups.SelectedItem = null;
        }

        private void add_user_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CreateFriend.xaml", UriKind.Relative));
        }

        private void create_group_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CreateGroup.xaml", UriKind.Relative));
        }

        private void logout_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Util.logout();
            NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
        }

        private void rate_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        async private void beer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                await CurrentApp.RequestProductPurchaseAsync(Constants.REMOVE_ADS_PRODUCT_ID, false);
                //check if purchase was made
                if (App.AdsRemoved)
                {
                    MessageBox.Show("Success!", "Thank you for your contribution. You might have to restart the app for the ads to fully disappear", MessageBoxButton.OK);
                    // notify marketplace that product has been delivered
                    CurrentApp.ReportProductFulfillment(Constants.REMOVE_ADS_PRODUCT_ID);
                }
            }
            catch (Exception exception)
            {
                if (App.isBeta)
                {
                    MessageBoxResult result = MessageBox.Show("An error has occured. Submit report?",
                                                                "IAP Error", MessageBoxButton.OKCancel);

                    if (result == MessageBoxResult.OK)
                    {
                        EmailComposeTask task = new EmailComposeTask();
                        task.To = "saurabh_arora129@hotmail.com";
                        task.Body = exception.Data.ToString() + "\n \n Inner Exception: \n" +
                                    exception.InnerException + "\n \n Message: \n" +
                                    exception.Message + "\n \n Stack Trace: \n" +
                                    exception.StackTrace;
                        task.Show();
                    }
                }
            }
        }

        private void account_settings_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AccountSettings.xaml", UriKind.Relative));
        }

        private void syncDatabaseBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            databaseSync.performSync();
        }

        private void _SyncConpleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    busyIndicator.IsRunning = false;
                    pageNo = 0;
                    populateData();
                    hasDataLoaded = true;
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
                        MessageBox.Show("Unable to sync with splitwise. You can continue to browse cached data", "Error", MessageBoxButton.OK);
                    }
                });
            }
        }

        private void resetAppPromo()
        {
            if (Util.isResetNeeded())
            {
                AppPromo.RateReminder reminder = new AppPromo.RateReminder();
                reminder.ResetCounters();
            }
        }
    }
}