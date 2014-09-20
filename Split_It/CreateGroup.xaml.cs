using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using Split_It_.Model;
using Split_It_.Controller;
using System.Windows.Media;
using System.Collections;
using System.Collections.ObjectModel;
using Split_It_.Utils;

namespace Split_It_
{
    public partial class CreateGroup : PhoneApplicationPage
    {
        BackgroundWorker createGroupBackgroundWorker;
        BackgroundWorker friendLoadingBackgroundWorker;

        ApplicationBarIconButton btnOkay;
        Group groupToAdd;

        ObservableCollection<User> groupMembers = new ObservableCollection<User>();
        ObservableCollection<User> friendsList = new ObservableCollection<User>();

        public CreateGroup()
        {
            InitializeComponent();
            (App.Current.Resources["PhoneRadioCheckBoxCheckBrush"] as SolidColorBrush).Color = Colors.Black;
            groupToAdd = new Group();

            createGroupBackgroundWorker = new BackgroundWorker();
            createGroupBackgroundWorker.WorkerSupportsCancellation = true;
            createGroupBackgroundWorker.DoWork += new DoWorkEventHandler(createGroupBackgroundWorker_DoWork);

            friendLoadingBackgroundWorker = new BackgroundWorker();
            friendLoadingBackgroundWorker.WorkerSupportsCancellation = true;
            friendLoadingBackgroundWorker.DoWork += new DoWorkEventHandler(friendLoadingBackgroundWorker_DoWork);
            friendLoadingBackgroundWorker.RunWorkerAsync();

            this.friendListPicker.ItemsSource = friendsList;
            this.friendListPicker.SummaryForSelectedItemsDelegate = this.FriendSummaryDelegate;

            groupMembers.Add(App.currentUser);
            createAppBar();
            llsFriends.ItemsSource = groupMembers;
        }

        private void friendLoadingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            QueryDatabase obj = new QueryDatabase();
            List<User> allFriends = obj.getAllFriends();
            Dispatcher.BeginInvoke(() =>
            {
                if (allFriends != null)
                {
                    foreach (var group in allFriends)
                    {
                        friendsList.Add(group);
                    }
                }
            });
            obj.closeDatabaseConnection();
        }

        protected void createAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;
            ApplicationBar.BackgroundColor = (Color)Application.Current.Resources["green"];
            ApplicationBar.ForegroundColor = Colors.White;

            btnOkay = new ApplicationBarIconButton();
            btnOkay.IconUri = new Uri("/Assets/Icons/save.png", UriKind.Relative);
            btnOkay.Text = "save";
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
            /*for (int i = 0; i < list.Count; i++)
            {
                // check if the last item has been reached so we don't put a "," at the end
                bool isLast = i == list.Count - 1;

                User friend = (User)list[i];
                summary = String.Concat(summary, friend.first_name);
                summary += isLast ? string.Empty : ", ";
            }*/
           
            if (list.Count != 0)
                summary = list.Count + " users selected";
            if (summary == String.Empty)
            {
                summary = "no friends selected";
            }
            return summary;
        }
        
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!createGroupBackgroundWorker.IsBusy)
            {
                busyIndicator.IsRunning = true;
                this.Focus();
                groupToAdd.members = groupMembers.ToList();
                createGroupBackgroundWorker.RunWorkerAsync();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void enableOkBtn()
        {
            if (!String.IsNullOrEmpty(tbName.Text))
                btnOkay.IsEnabled = true;
            else
                btnOkay.IsEnabled = false;
        }

        private void createGroupBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ModifyDatabase modify = new ModifyDatabase(_createGroupCompleted);
            modify.createGroup(groupToAdd);
        }

        private void _createGroupCompleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    Group group = PhoneApplicationService.Current.State[Constants.NEW_GROUP] as Group;
                    //App.groupsList.Add(group);
                    PhoneApplicationService.Current.State[Constants.NEW_GROUP] = null;
                    busyIndicator.IsRunning = false;
                    NavigationService.GoBack();
                });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    busyIndicator.IsRunning = false;
                    MessageBox.Show("Unable to create group", "Error", MessageBoxButton.OK);
                });
            }
        }

        private void tbName_TextChanged(object sender, TextChangedEventArgs e)
        {
            groupToAdd.name = tbName.Text;
            enableOkBtn();
        }

        private void friendListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            groupMembers.Clear();
            groupMembers.Add(App.currentUser);

            if (this.friendListPicker.SelectedItems == null)
                return;
            foreach (var item in this.friendListPicker.SelectedItems)
            {
                User user = item as User;
                groupMembers.Add(user);
            }
        }
    }
}