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

namespace Split_It_
{
    public partial class MainPage : PhoneApplicationPage
    {
        ObservableCollection<User> friendsList = new ObservableCollection<User>();
        ObservableCollection<Group> groupsList = new ObservableCollection<Group>();
        ObservableCollection<Expense> expensesList = new ObservableCollection<Expense>();

        public MainPage()
        {
            InitializeComponent();

            App.accessToken = Util.getAccessToken();
            App.accessTokenSecret = Util.getAccessTokenSecret();

            llsFriends.ItemsSource = friendsList;
            llsExpenses.ItemsSource = expensesList;
            llsGroups.ItemsSource = groupsList;

            populateData();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            String firstUse;

            //This condition will only be true if the user has launched this page. This paramter (afterLogin) wont be there
            //if the page has been accessed from the back stack
            if (NavigationContext.QueryString.TryGetValue("afterLogin", out firstUse))
            {
                SyncDatabase databaseSync;
                if (firstUse.Equals("true"))
                    databaseSync = new SyncDatabase(_SyncConpleted, true);
                else
                    databaseSync = new SyncDatabase(_SyncConpleted, false);

                databaseSync.performSync();
            }
        }

        private void populateData()
        {
            QueryDatabase obj = new QueryDatabase();
            foreach (var friend in obj.getFriends())
            {
                friendsList.Add(friend);
            }
        }

        private void _SyncConpleted(bool success)
        {
            if (success)
            {
                emptyData();
                populateData();
            }
        }

        private void emptyData()
        {
            friendsList.Clear();
            groupsList.Clear();
            expensesList.Clear();
        }
    }
}