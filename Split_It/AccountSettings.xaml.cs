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
using Split_It_.Controller;
using System.ComponentModel;
using Split_It_.Model;
using System.Collections.ObjectModel;
using System.Collections;

namespace Split_It_
{
    public partial class AccountSettings : PhoneApplicationPage
    {
        BackgroundWorker getSupportedCurrenciesBackgroundWorker;
        BackgroundWorker editUserBackgroundWorker;
        protected ObservableCollection<Currency> currenciesList = new ObservableCollection<Currency>();
        User currentUser;
        bool currencyModified = false;

        public AccountSettings()
        {
            InitializeComponent();

            getSupportedCurrenciesBackgroundWorker = new BackgroundWorker();
            getSupportedCurrenciesBackgroundWorker.WorkerSupportsCancellation = true;
            getSupportedCurrenciesBackgroundWorker.DoWork += new DoWorkEventHandler(getSupportedCurrenciesBackgroundWorker_DoWork);
            getSupportedCurrenciesBackgroundWorker.RunWorkerAsync();

            editUserBackgroundWorker = new BackgroundWorker();
            editUserBackgroundWorker.WorkerSupportsCancellation = true;
            editUserBackgroundWorker.DoWork += new DoWorkEventHandler(editUserBackgroundWorker_DoWork);

            this.DataContext = App.currentUser;
            this.currencyListPicker.ItemsSource = currenciesList;
            this.currencyListPicker.SummaryForSelectedItemsDelegate = this.CurrencySummaryDelegate;
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

            ApplicationBarIconButton btnOkay = new ApplicationBarIconButton();
            btnOkay.IconUri = new Uri("/Assets/Icons/save.png", UriKind.Relative);
            btnOkay.Text = "save";
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
            if (canProceed())
            {
                busyIndicator.IsRunning = true;
                currentUser = new User();
                currentUser.id = App.currentUser.id;
                currentUser.first_name = tbFirstName.Text;
                currentUser.last_name = tbLastName.Text;
                currentUser.email = tbEmail.Text;
                Currency selectedCurrency = (currencyListPicker.SelectedItem as Currency);

                if (selectedCurrency != null)
                    currentUser.default_currency = selectedCurrency.currency_code;

                if (!currentUser.default_currency.Equals(App.currentUser.default_currency))
                    currencyModified = true;

                editUserBackgroundWorker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Email and First Name cannot be empty", "Error", MessageBoxButton.OK);
            }
        }

        private bool canProceed()
        {
            return !(String.IsNullOrEmpty(tbEmail.Text) || String.IsNullOrEmpty(tbFirstName.Text));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void editUserBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Request.UpdateUserRequest request = new Request.UpdateUserRequest(currentUser);
            request.updateUser(_editUserCompleted);
        }

        private void _editUserCompleted(User updatedUserDetails, HttpStatusCode statusCode)
        {
            if (updatedUserDetails != null && statusCode == HttpStatusCode.OK)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    App.currentUser = updatedUserDetails;
                    busyIndicator.IsRunning = false;
                    showPromptAndGoBack();
                });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    busyIndicator.IsRunning = false;

                    if (statusCode == HttpStatusCode.Unauthorized)
                    {
                        Split_It_.Utils.Util.logout();
                        NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show("Unable to edit user details.", "Error", MessageBoxButton.OK);
                    }
                });
            }
        }

        private void showPromptAndGoBack()
        {
            if(currencyModified)
                MessageBox.Show("In order to set exisiting expenses to new default currency, please go to Account Settings on Splitwise.com", "Success", MessageBoxButton.OK);
            NavigationService.GoBack();
        }

        private void getSupportedCurrenciesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Currency defaultCurrency = null;
            QueryDatabase query = new QueryDatabase();
            foreach (var item in query.getSupportedCurrencies())
            {
                if (item.currency_code == App.currentUser.default_currency)
                    defaultCurrency = item;

                Dispatcher.BeginInvoke(() =>
                {
                    currenciesList.Add(item);
                });

                if (defaultCurrency == null && item.currency_code == "USD")
                    defaultCurrency = item;
            }
            Dispatcher.BeginInvoke(() =>
            {
                this.currencyListPicker.SelectedItem = defaultCurrency;
            });
        }

        private object CurrencySummaryDelegate(IList list)
        {
            string summary = String.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                // check if the last item has been reached so we don't put a "," at the end
                bool isLast = i == list.Count - 1;

                Currency currency = (Currency)list[i];
                summary = String.Concat(summary, currency.currency_code);
                summary += isLast ? string.Empty : ", ";
            }
            if (String.IsNullOrEmpty(summary))
            {
                summary = "select currency";
            }
            return summary;
        }
    }
}