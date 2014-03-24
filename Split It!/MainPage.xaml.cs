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

namespace Split_It_
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            App.accessToken = Util.getAccessToken();
            App.accessTokenSecret = Util.getAccessTokenSecret();

            CurrentUserRequest request = new CurrentUserRequest();
            request.getCurrentUser();
        }
    }
}