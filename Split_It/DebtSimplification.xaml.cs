using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace Split_It_
{
    public partial class DebtSimplification : PhoneApplicationPage
    {
        CheckBox doNotShowCheckBox;

        public DebtSimplification()
        {
            InitializeComponent();
            Uri url = new Uri("https://secure.splitwise.com/users/simplify_debts");
            browser.Navigate(url);
            createAppBar();
        }

        private void createAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;
            ApplicationBar.BackgroundColor = (Color)Application.Current.Resources["green"];
            ApplicationBar.ForegroundColor = Colors.White;

            //Add expense to group
            ApplicationBarIconButton btnDone = new ApplicationBarIconButton();
            btnDone.IconUri = new Uri("/Assets/Icons/ok.png", UriKind.Relative);
            btnDone.Text = "done";
            ApplicationBar.Buttons.Add(btnDone);
            btnDone.Click += new EventHandler(btnDone_Click);
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void browser_Navigating(object sender, NavigatingEventArgs e)
        {
            busyIndicator.IsRunning = true;
        }

        private void browser_Navigated(object sender, NavigationEventArgs e)
        {
            busyIndicator.IsRunning = false;
            /*if (e.Uri.ToString().Contains("account/settings"))
            {
                CustomMessageBox box = new CustomMessageBox()
                {
                    Message = "Debt simplification was successful",
                    Caption = "Success",
                    LeftButtonContent = "Okay"
                };
                box.Dismissed += (s1, e1) =>
                {
                    if (NavigationService.CanGoBack)
                        NavigationService.GoBack();
                };

            }*/
        }

        private void browser_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            busyIndicator.IsRunning = false;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Utils.Util.doNotShowDebtSimplificationBox())
                return;

            string hey = "Hey there!";
            string message = "After you are done with debt simplification, refresh the app to reflect the changes.";
            //MessageBox.Show(message, hey, MessageBoxButton.OK);

            doNotShowCheckBox = new CheckBox()
            {
                Content = "Do not show again",
                IsChecked = false
            };

            CustomMessageBox box = new CustomMessageBox()
            {
                Message = message,
                Caption = hey,
                Content = doNotShowCheckBox,
                LeftButtonContent = "Okay"
            };
            box.Dismissed += box_Dismissed;
            box.Show();
        }

        void box_Dismissed(object sender, DismissedEventArgs e)
        {
            if (doNotShowCheckBox.IsChecked.Value)
            {
                Utils.Util.setDonNotShowDebtSimplifationBox();
            }
        }
    }
}