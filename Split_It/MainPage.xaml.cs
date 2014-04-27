﻿using System;
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

        private void populateData()
        {
            emptyData();
            QueryDatabase obj = new QueryDatabase();
            foreach (var friend in obj.getAllFriends())
            {
                friendsList.Add(friend);
            }

            foreach (var expense in obj.getAllExpenses())
            {
                expensesList.Add(expense);
            }

            obj.closeDatabaseConnection();
        }

        private void _SyncConpleted(bool success)
        {
            if (success)
            {
                populateData();
            }
        }

        private void emptyData()
        {
            friendsList.Clear();
            groupsList.Clear();
            expensesList.Clear();
        }

        private void llsFriends_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            User selectedUser = selector.SelectedItem as User;
            NavigationService.Navigate(new Uri("/UserDetails.xaml", UriKind.Relative));
        }
    }
}