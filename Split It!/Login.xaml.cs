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

namespace Split_It_
{
    public partial class Login : PhoneApplicationPage
    {
        private OAuthRequest request;
        public Login()
        {
            InitializeComponent();
            request = new OAuthRequest();

            request.getReuqestToken(_requestTokenRetrieved);
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

        }
    }
}