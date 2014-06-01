using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Split_It_.Utils;
using Split_It_.Request;
using RestSharp;
using RestSharp.Authenticators;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using System.IO.IsolatedStorage;
using Windows.ApplicationModel;

namespace Split_It_
{
    public partial class Login : PhoneApplicationPage
    {
        private OAuthRequest request;
        
        public Login()
        {
            InitializeComponent();

            //copy database (if needed) in the background
            Task.Run(async () => await CopyDatabase());

            request = new OAuthRequest();
            request.getReuqestToken(_requestTokenRetrieved);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            //do not allow him to go back to the Splash Page. therefore clear the back stack
            while (NavigationService.CanGoBack) NavigationService.RemoveBackEntry();
        }

        private async Task CopyDatabase()
        {
            bool isDatabaseExisting = false;

            try
            {
                StorageFile storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(Constants.DATABASE_NAME);
                isDatabaseExisting = true;
            }
            catch
            {
                isDatabaseExisting = false;
            }

            if (!isDatabaseExisting)
            {
                StorageFile databaseFile = await Package.Current.InstalledLocation.GetFileAsync(Constants.DATABASE_NAME);
                await databaseFile.CopyAsync(ApplicationData.Current.LocalFolder);
            }
        }

        private void _requestTokenRetrieved(Uri uri)
        {
            loginBrowser.Navigate(uri);
        }


        private void loginBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.AbsoluteUri.Contains(Constants.OAUTH_CALLBACK))
            {
                var arguments = e.Uri.AbsoluteUri.Split('?');
                if (arguments.Length < 1)
                    return;
                request.getAccessToken(arguments[1], _onAccessTokenReceived);
            }

            busyIndicator.IsRunning = true;
        }

        private void _onAccessTokenReceived(string accessToken, string accessTokenSecret)
        {
            Util.setAccessToken(accessToken);
            Util.setAccessTokenSecret(accessTokenSecret);
            NavigationService.Navigate(new Uri("/SplashPage.xaml?afterLogin=true", UriKind.Relative));
        }

        private void loginBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            busyIndicator.IsRunning = false;
        }

        private void loginBrowser_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            busyIndicator.IsRunning = false;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            string hey = "Hey there!";
            string message = "Thank you for downloading Split It, a splitwise client for windows phone. Please login to splitwise below and authorize the app to access your account securely.";

            MessageBox.Show(message, hey, MessageBoxButton.OK);
        }
    }
}