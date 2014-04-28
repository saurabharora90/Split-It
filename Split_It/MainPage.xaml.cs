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

namespace Split_It_
{
    public partial class MainPage : PhoneApplicationPage
    {
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

        public MainPage()
        {
            InitializeComponent();

            App.accessToken = Util.getAccessToken();
            App.accessTokenSecret = Util.getAccessTokenSecret();

            llsFriends.ItemsSource = friendsList;
            llsExpenses.ItemsSource = expensesList;
            llsGroups.ItemsSource = groupsList;

            dataLoadingBackgroundWorker = new BackgroundWorker();
            dataLoadingBackgroundWorker.WorkerSupportsCancellation = true;
            dataLoadingBackgroundWorker.DoWork += new DoWorkEventHandler(dataLoadingBackgroundWorker_DoWork);

            syncDatabaseBackgroundWorker = new BackgroundWorker();
            syncDatabaseBackgroundWorker.WorkerSupportsCancellation = true;
            syncDatabaseBackgroundWorker.DoWork += new DoWorkEventHandler(syncDatabaseBackgroundWorker_DoWork);

            populateData();
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
            QueryDatabase obj = new QueryDatabase();
            foreach (var friend in obj.getAllFriends())
            {
                friendsList.Add(friend);
            }
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

                populateData();
            });
            }
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

        private void llsExpenses_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            lock (o)
            {
                if (dataLoadingBackgroundWorker.IsBusy != true && morePages)
                {
                    pageNo++;
                    dataLoadingBackgroundWorker.RunWorkerAsync();
                }
            }
        }
    }
}