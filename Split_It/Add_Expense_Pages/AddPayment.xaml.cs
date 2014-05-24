using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Split_It_.Model;
using Split_It_.Utils;
using System.Windows.Media;
using System.ComponentModel;
using Split_It_.Controller;

namespace Split_It_.Add_Expense_Pages
{
    public partial class AddPayment : PhoneApplicationPage
    {
        User paymentUser;
        BackgroundWorker addPaymentBackgroundWorker;
        public double transferAmount { get; set; }
        public string currency { get; set; }
        public string details { get; set; }

        public AddPayment()
        {
            InitializeComponent();

            paymentUser = PhoneApplicationService.Current.State[Constants.PAYMENT_TO_USER] as User;
            addPaymentBackgroundWorker = new BackgroundWorker();
            addPaymentBackgroundWorker.WorkerSupportsCancellation = true;
            addPaymentBackgroundWorker.DoWork += new DoWorkEventHandler(addPaymentBackgroundWorker_DoWork);

            createAppBar();
            setupData();

            toUser.DataContext = paymentUser;
            fromUser.DataContext = App.currentUser;
        }

        private void createAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;
            ApplicationBar.BackgroundColor = (Color)Application.Current.Resources["green"];
            ApplicationBar.ForegroundColor = Colors.White;

            //Settle up button
            ApplicationBarIconButton btnOk = new ApplicationBarIconButton();
            btnOk.IconUri = new Uri("/Assets/Icons/ok.png", UriKind.Relative);
            btnOk.Text = "ok";
            btnOk.Click += new EventHandler(btnOk_Click);

            ApplicationBarIconButton btnCanel = new ApplicationBarIconButton();
            btnCanel.IconUri = new Uri("/Assets/Icons/cancel.png", UriKind.Relative);
            btnCanel.Text = "cancel";
            btnCanel.Click += new EventHandler(btnCanel_Click);

            ApplicationBar.Buttons.Add(btnOk);
            ApplicationBar.Buttons.Add(btnCanel);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            transferAmount = Convert.ToDouble(tbAmount.Text);
            currency = tbCurrency.Text;
            details = tbDetails.Text;

            if (addPaymentBackgroundWorker.IsBusy != true && transferAmount!=0 && !String.IsNullOrEmpty(currency))
            {
                SystemTray.ProgressIndicator = new ProgressIndicator();
                SystemTray.ProgressIndicator.IsIndeterminate = true;
                SystemTray.ProgressIndicator.IsVisible = true;

                addPaymentBackgroundWorker.RunWorkerAsync();
            }
        }

        private void btnCanel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void setupData()
        {
            Balance_User defaultBalance = Util.getDefaultBalance(paymentUser.balance);
            transferAmount = System.Convert.ToDouble(defaultBalance.amount);
            currency = defaultBalance.currency_code;

            tbCurrency.Text = currency;
            tbAmount.Text = Math.Abs(transferAmount).ToString();

            DateTime now = DateTime.UtcNow;
            string dateString = now.ToString("dd MMMM, yyyy", System.Globalization.CultureInfo.InvariantCulture);
            tbDate.Text = "on " + dateString;
        }

        private void addPaymentBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //only setup the needed details.
            Expense paymentExpense = new Expense();
            paymentExpense.payment = true;
            paymentExpense.cost = transferAmount.ToString();
            paymentExpense.currency_code = currency;
            paymentExpense.creation_method = "payment";
            paymentExpense.description = "Payment";
            paymentExpense.details = details;
            paymentExpense.users = new List<Expense_Share>();

            Expense_Share fromUser = new Expense_Share();
            fromUser.user_id = App.currentUser.id;
            fromUser.owed_share = "0";
            fromUser.paid_share = transferAmount.ToString();

            Expense_Share toUser = new Expense_Share();
            toUser.user_id = paymentUser.id;
            toUser.paid_share = "0";
            toUser.owed_share = transferAmount.ToString();

            paymentExpense.users.Add(fromUser);
            paymentExpense.users.Add(toUser);

            ModifyDatabase modify = new ModifyDatabase(_recordPaymentCompleted);
            modify.addExpense(paymentExpense);
        }

        private void _recordPaymentCompleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (SystemTray.ProgressIndicator != null)
                        SystemTray.ProgressIndicator.IsVisible = false;

                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (SystemTray.ProgressIndicator != null)
                        SystemTray.ProgressIndicator.IsVisible = false;

                    if (errorCode == HttpStatusCode.Unauthorized)
                    {
                        Util.logout();
                        NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show("Unable to record payment", "Error", MessageBoxButton.OK);
                    }
                });
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            tbAmount.Focus();
        }
    }
}