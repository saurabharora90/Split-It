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
using System.Threading.Tasks;
using Windows.Storage;
using Windows.ApplicationModel;
using Split_It_.Controller;
using System.IO;
using System.IO.IsolatedStorage;

namespace Split_It_
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            App.accessToken = Util.getAccessToken();
            App.accessTokenSecret = Util.getAccessTokenSecret();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            String firstUse;
            if (NavigationContext.QueryString.TryGetValue("afterLogin", out firstUse))
            {
                if (firstUse.Equals("true"))
                {
                    SyncDatabase setutFirstUse = new SyncDatabase(null, true);
                    setutFirstUse.performSync();
                }
                else
                {
                    //load details from database
                }
            }
        }
    }
}