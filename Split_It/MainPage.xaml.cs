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
        BackgroundWorker bw;

        public MainPage()
        {
            InitializeComponent();

            App.accessToken = Util.getAccessToken();
            App.accessTokenSecret = Util.getAccessTokenSecret();

            llsFriends.ItemsSource = friendsList;
            llsExpenses.ItemsSource = expensesList;
            llsGroups.ItemsSource = groupsList;

            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
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
                SyncDatabase databaseSync;
                if (firstUse.Equals("true"))
                {
                    //do not allow him to go back to the Login Page. therefore clear the back stack
                    while (NavigationService.CanGoBack) NavigationService.RemoveBackEntry();
                    databaseSync = new SyncDatabase(_SyncConpleted, true);
                }
                else
                    databaseSync = new SyncDatabase(_SyncConpleted, false);

                databaseSync.performSync();
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            loadExpensesAndGroups();
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
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
        }

        private void loadExpensesAndGroups()
        {
            //the rest of the work is done in a backgroundworker
            QueryDatabase obj = new QueryDatabase();
            List<Expense> allExpenses = obj.getAllExpenses();

            Dispatcher.BeginInvoke(() =>
            {
                expensesList.Clear();
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
                if (bw.WorkerSupportsCancellation == true)
                    bw.CancelAsync();

                populateData();
            }
        }

        private void llsFriends_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            User selectedUser = selector.SelectedItem as User;
            NavigationService.Navigate(new Uri("/UserDetails.xaml", UriKind.Relative));
        }
    }
}