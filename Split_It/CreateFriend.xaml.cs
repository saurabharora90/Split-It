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
using System.Windows.Media;
using System.Text.RegularExpressions;
using Microsoft.Phone.Tasks;
using Split_It_.Controller;

namespace Split_It_
{
    public partial class CreateFriend : PhoneApplicationPage
    {
        BackgroundWorker createFriendBackgroundWorker;
        ApplicationBarIconButton btnOkay;
        string email, firstName, lastName;

        public CreateFriend()
        {
            InitializeComponent();

            createFriendBackgroundWorker = new BackgroundWorker();
            createFriendBackgroundWorker.WorkerSupportsCancellation = true;
            createFriendBackgroundWorker.DoWork += new DoWorkEventHandler(createFriendBackgroundWorker_DoWork);
            
            createAppBar();
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

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!createFriendBackgroundWorker.IsBusy)
            {
                busyIndicator.IsRunning = true;
                this.Focus();
                email = tbEmail.Text;
                firstName = tbFirstName.Text;
                lastName = tbLastName.Text;
                createFriendBackgroundWorker.RunWorkerAsync();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void enableOkBtn()
        {
            if (Utils.Util.IsValidEmail(tbEmail.Text) && !String.IsNullOrEmpty(tbFirstName.Text))
                btnOkay.IsEnabled = true;
            else
                btnOkay.IsEnabled = false;
        }
        
        private void createFriendBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ModifyDatabase modify = new ModifyDatabase(_addFriendCompleted);
            modify.createFriend(email, firstName, lastName);
        }

        private void tbEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            enableOkBtn();
        }

        private void btnImport_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailAddressChooserTask emailAddressChooserTask = new EmailAddressChooserTask();
            emailAddressChooserTask.Completed += new EventHandler<EmailResult>(emailAddressChooserTask_Completed);
            emailAddressChooserTask.Show();
        }

        void emailAddressChooserTask_Completed(object sender, EmailResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                tbEmail.Text = e.Email;
                tbFirstName.Text = e.DisplayName;
            }
        }

        private void _addFriendCompleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    busyIndicator.IsRunning = false;
                    NavigationService.GoBack();
                });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    busyIndicator.IsRunning = false;
                    MessageBox.Show("Unable to add user", "Error", MessageBoxButton.OK);
                });
            }
        }

        private void tbFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            enableOkBtn();
        }
    }
}