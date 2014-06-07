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
using Split_It_.Controller;
using Split_It_.Utils;

namespace Split_It_
{
    public partial class SplashPage : PhoneApplicationPage
    {
        BackgroundWorker syncDatabaseBackgroundWorker;
        SyncDatabase databaseSync;

        public SplashPage()
        {
            InitializeComponent();
            App.accessToken = Util.getAccessToken();
            App.accessTokenSecret = Util.getAccessTokenSecret();

            syncDatabaseBackgroundWorker = new BackgroundWorker();
            syncDatabaseBackgroundWorker.WorkerSupportsCancellation = true;
            syncDatabaseBackgroundWorker.DoWork += new DoWorkEventHandler(syncDatabaseBackgroundWorker_DoWork);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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
                    if (SystemTray.ProgressIndicator != null)
                        SystemTray.ProgressIndicator.IsVisible = false;

                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (SystemTray.ProgressIndicator != null)
                        SystemTray.ProgressIndicator.IsVisible = false;

                    if (errorCode == HttpStatusCode.Unauthorized)
                    {
                        Util.logout();
                        NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show("Unable to sync with splitwise. You can continue to browse cached data", "Error", MessageBoxButton.OK);
                        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    }
                });
            }
        }
    }
}