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

        //Use a BackgroundWorker to load data from database (except for friends) as expenses
        //and groups are time consuming operations
        BackgroundWorker expenseLoadingBackgroundWorker;
        BackgroundWorker groupLoadingBackgroundWorker;
       
        private object o = new object();
        private int pageNo = 0;
        private bool morePages = true;

        private ApplicationBarMenuItem btnAllFriends, btnBalanceFriends, btnYouOweFriends, btnOwesYouFriends;
        private double postiveBalance = 0, negativeBalance = 0, totalBalance = 0;
        private NetBalances netBalanceObj = new NetBalances();

        public MainPage()
        {
            InitializeComponent();

            App.friendsList = new ObservableCollection<User>();
            App.groupsList = new ObservableCollection<Group>();
            App.expensesList = new ObservableCollection<Expense>();

            llsFriends.ItemsSource = balanceFriends;
            llsExpenses.ItemsSource = App.expensesList;
            llsGroups.ItemsSource = App.groupsList;

            expenseLoadingBackgroundWorker = new BackgroundWorker();
            expenseLoadingBackgroundWorker.WorkerSupportsCancellation = true;
            expenseLoadingBackgroundWorker.DoWork += new DoWorkEventHandler(expenseLoadingBackgroundWorker_DoWork);

            groupLoadingBackgroundWorker = new BackgroundWorker();
            groupLoadingBackgroundWorker.WorkerSupportsCancellation = true;
            groupLoadingBackgroundWorker.DoWork += new DoWorkEventHandler(groupLoadingBackgroundWorker_DoWork);

            setupAppBars();
            populateData();
        }

        private void setupAppBars()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;

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
        
        private void btnAllFriends_Click(object sender, EventArgs e)
        {
            llsFriends.ItemsSource = App.friendsList;
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
            else
                //do not allow him to go back to the Splash Page. therefore clear the back stack
                while (NavigationService.CanGoBack) NavigationService.RemoveBackEntry();
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
            App.friendsList.Clear();
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
                App.friendsList.Add(friend);
                Balance_User defaultBalance = Util.getDefaultBalance(friend.balance);
                double balance = System.Convert.ToDouble(defaultBalance.amount);
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

            netBalanceObj.setBalances(App.currentUser.default_currency, totalBalance, postiveBalance, negativeBalance);
            btnOwesYouFriends.Text = netBalanceObj.PositiveBalance;
            btnYouOweFriends.Text = netBalanceObj.NegativeBalance;
            btnBalanceFriends.Text = netBalanceObj.NetBalance;
            obj.closeDatabaseConnection();
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
                        App.expensesList.Clear();

                    if (allExpenses == null || allExpenses.Count == 0)
                        morePages = false;
                    else
                        morePages = true;

                    if (allExpenses != null)
                    {
                        foreach (var expense in allExpenses)
                        {
                            App.expensesList.Add(expense);
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
                if (allGroups != null)
                {
                    foreach (var group in allGroups)
                    {
                        App.groupsList.Add(group);
                    }
                }
            });
            obj.closeDatabaseConnection();
        }

        private void llsExpenses_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            lock (o)
            {
                Expense expense = e.Container.Content as Expense;
                if (expense != null)
                {
                    int offset = 2;

                    if (expenseLoadingBackgroundWorker.IsBusy != true && morePages && App.expensesList.Count - App.expensesList.IndexOf(expense) <= offset)
                    {
                        pageNo++;
                        expenseLoadingBackgroundWorker.RunWorkerAsync(false);
                    }
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

            ApplicationBar.BackgroundColor = (Color)Application.Current.Resources["green"];
            ApplicationBar.ForegroundColor = Colors.White;
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
    }
}