using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Split_It_.UserControls
{
    public partial class AdControl : UserControl
    {
        public AdControl()
        {
            InitializeComponent();
            MSAdControl.ErrorOccurred += MSAdControl_ErrorOccurred;
            MSAdControl.AdRefreshed += new EventHandler(MSAdControl_NewAd);
        }

        void MSAdControl_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                MSAdControl.Visibility = Visibility.Collapsed;
                AdDuplexAdControl.Visibility = Visibility.Visible;
            });
        }

        void MSAdControl_NewAd(object sender, EventArgs e)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                AdDuplexAdControl.Visibility = Visibility.Collapsed;
                MSAdControl.Visibility = Visibility.Visible;
            });
        }
    }
}
