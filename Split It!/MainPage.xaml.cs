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

namespace Split_It_
{
    public partial class MainPage : PhoneApplicationPage
    {
        List<User> friendsList;
        List<Group> groupsList;
        List<Expense> expensesList;

        public MainPage()
        {
            InitializeComponent();

            App.accessToken = Util.getAccessToken();
            App.accessTokenSecret = Util.getAccessTokenSecret();

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
                    databaseSync = new SyncDatabase(null, true);
                else
                    databaseSync = new SyncDatabase(null, false);

                databaseSync.performSync();
            }
        }

        private void populateData()
        {
            QueryDatabase obj = new QueryDatabase();
            friendsList = obj.getFriends();
        }
    }
}