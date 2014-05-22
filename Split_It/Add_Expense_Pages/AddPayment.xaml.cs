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

namespace Split_It_.Add_Expense_Pages
{
    public partial class AddPayment : PhoneApplicationPage
    {
        User paymentUser;
        public double transferAmount { get; set; }
        public string currency { get; set; }

        public AddPayment()
        {
            InitializeComponent();

            paymentUser = PhoneApplicationService.Current.State[Constants.PAYMENT_TO_USER] as User;
            
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

            ApplicationBarIconButton btnCanel = new ApplicationBarIconButton();
            btnCanel.IconUri = new Uri("/Assets/Icons/cancel.png", UriKind.Relative);
            btnCanel.Text = "cancel";

            ApplicationBar.Buttons.Add(btnOk);
            ApplicationBar.Buttons.Add(btnCanel);
        }

        private void setupData()
        {
            Balance_User defaultBalance = Util.getDefaultBalance(paymentUser.balance);
            transferAmount = System.Convert.ToDouble(defaultBalance.amount);
            currency = defaultBalance.currency_code;
        }
    }
}