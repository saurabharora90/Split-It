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

        /*private async Task CopyDatabase()
        {
            StorageFile dbFile = null;
            try
            {
                // Try to get the 
                dbFile = await StorageFile.GetFileFromPathAsync(Constants.DB_PATH);
            }
            catch (FileNotFoundException)
            {
                if (dbFile == null)
                {
                    // Copy file from installation folder to local folder.
                    // Obtain the virtual store for the application.
                    IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();

                    // Create a stream for the file in the installation folder.
                    using (Stream input = Application.GetResourceStream(new Uri(Constants.DATABASE_NAME, UriKind.Relative)).Stream)
                    {
                        // Create a stream for the new file in the local folder.
                        using (IsolatedStorageFileStream output = iso.CreateFile(Constants.DB_PATH))
                        {
                            // Initialize the buffer.
                            byte[] readBuffer = new byte[4096];
                            int bytesRead = -1;

                            // Copy the file from the installation folder to the local folder. 
                            while ((bytesRead = input.Read(readBuffer, 0, readBuffer.Length)) > 0)
                            {
                                output.Write(readBuffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
        }*/

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