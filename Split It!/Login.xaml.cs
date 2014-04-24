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
        }

        private void _onAccessTokenReceived(string accessToken, string accessTokenSecret)
        {
            Util.setAccessToken(accessToken);
            Util.setAccessTokenSecret(accessTokenSecret);
            NavigationService.Navigate(new Uri("/MainPage.xaml?afterLogin=true", UriKind.Relative));
        }
    }
}