﻿using System;
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
using System.Globalization;

namespace Split_It_.Add_Expense_Pages
{
    public partial class AddPayment : PhoneApplicationPage
    {
        User paymentUser;
        BackgroundWorker addPaymentBackgroundWorker;
        public double transferAmount { get; set; }
        public string currency { get; set; }
        public string details { get; set; }

        int paymentType;

        string decimalsep = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;

        public AddPayment()
        {
            InitializeComponent();

            paymentUser = PhoneApplicationService.Current.State[Constants.PAYMENT_USER] as User;
            paymentType = Convert.ToInt32(PhoneApplicationService.Current.State[Constants.PAYMENT_TYPE]);
            
            addPaymentBackgroundWorker = new BackgroundWorker();
            addPaymentBackgroundWorker.WorkerSupportsCancellation = true;
            addPaymentBackgroundWorker.DoWork += new DoWorkEventHandler(addPaymentBackgroundWorker_DoWork);

            setupData();
            createAppBar();

            if (paymentType == Constants.PAYMENT_TO)
            {
                toUser.DataContext = paymentUser;
                fromUser.DataContext = App.currentUser;
            }
            else
            {
                toUser.DataContext = App.currentUser;
                fromUser.DataContext = paymentUser;
            }
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
            btnOk.IconUri = new Uri("/Assets/Icons/save.png", UriKind.Relative);
            btnOk.Text = "save";
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
            //to hide the keyboard if any
            this.Focus();

            try
            {
                transferAmount = Convert.ToDouble(tbAmount.Text);
            }
            catch (FormatException exception)
            {
                return;
            }
            currency = tbCurrency.Text;
            details = tbDetails.Text;

            if (addPaymentBackgroundWorker.IsBusy != true && transferAmount!=0 && !String.IsNullOrEmpty(currency))
            {
                busyIndicator.Content = "";
                busyIndicator.IsRunning = true;

                addPaymentBackgroundWorker.RunWorkerAsync();
            }
        }

        protected void tbAmount_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            //do not allow user to input more than one decimal point
            if (textBox.Text.Contains(decimalsep) && (e.PlatformKeyCode == 190 || e.PlatformKeyCode == 188))
                e.Handled = true;
        }

        private void btnCanel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void setupData()
        {
            Balance_User defaultBalance = getPaymentAmount();
            transferAmount = System.Convert.ToDouble(defaultBalance.amount, System.Globalization.CultureInfo.InvariantCulture);
            currency = defaultBalance.currency_code;

            tbCurrency.Text = currency;
            tbAmount.Text = String.Format("{0:0.00}", Math.Abs(transferAmount));

            DateTime now = DateTime.UtcNow;
            string dateString = now.ToString("dd MMMM, yyyy", System.Globalization.CultureInfo.InvariantCulture);
            tbDate.Text = "on " + dateString;
        }

        private Balance_User getPaymentAmount()
        {
            foreach (var balance in paymentUser.balance)
            {
                if (paymentType == Constants.PAYMENT_TO)
                {
                    if (System.Convert.ToDouble(balance.amount, System.Globalization.CultureInfo.InvariantCulture) < 0)
                        return balance;
                }
                else
                {
                    if (System.Convert.ToDouble(balance.amount, System.Globalization.CultureInfo.InvariantCulture) > 0)
                        return balance;
                }
            }

            return null;
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
            Expense_Share toUser = new Expense_Share();
            if (paymentType == Constants.PAYMENT_TO)
            {
                fromUser.user_id = App.currentUser.id;
                fromUser.owed_share = "0";
                fromUser.paid_share = transferAmount.ToString();

                toUser.user_id = paymentUser.id;
                toUser.paid_share = "0";
                toUser.owed_share = transferAmount.ToString();
            }
            else
            {
                toUser.user_id = App.currentUser.id;
                toUser.paid_share = "0";
                toUser.owed_share = transferAmount.ToString();

                fromUser.user_id = paymentUser.id;
                fromUser.paid_share = transferAmount.ToString();
                fromUser.owed_share = "0";
            }

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
                    busyIndicator.IsRunning = false;
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    busyIndicator.IsRunning = false;

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
            tbAmount.Select(tbAmount.Text.Length, 0);
        }
    }
}