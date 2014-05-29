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
using Split_It_.Model;
using System.Windows.Media;
using Split_It_.Controller;
using System.ComponentModel;

namespace Split_It_.Add_Expense_Pages
{
    public partial class SplitUnequally : PhoneApplicationPage
    {
        Expense expenseToAdd;
        BackgroundWorker addExpenseBackgroundWorker;

        public SplitUnequally()
        {
            InitializeComponent();

            expenseToAdd = PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] as Expense;
            addExpenseBackgroundWorker = new BackgroundWorker();
            addExpenseBackgroundWorker.WorkerSupportsCancellation = true;
            addExpenseBackgroundWorker.DoWork += new DoWorkEventHandler(addExpenseBackgroundWorker_DoWork);

            createAppBar();
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

            ApplicationBarIconButton btnOkay = new ApplicationBarIconButton();
            btnOkay.IconUri = new Uri("/Assets/Icons/ok.png", UriKind.Relative);
            btnOkay.Text = "ok";
            btnOkay.IsEnabled = true;
            ApplicationBar.Buttons.Add(btnOkay);
            btnOkay.Click += new EventHandler(btnOk_Click);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //to hide the keyboard if any
            this.Focus();

            PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] = expenseToAdd;
            NavigationService.Navigate(new Uri("/Add_Expense_Pages/SplitUnequally.xaml", UriKind.Relative));

            if (addExpenseBackgroundWorker.IsBusy != true)
            {
                busyIndicator.Content = "adding expense";
                busyIndicator.IsRunning = true;

                addExpenseBackgroundWorker.RunWorkerAsync();
            }
        }

        private void addExpenseBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ModifyDatabase modify = new ModifyDatabase(_addExpenseCompleted);
            modify.addExpense(expenseToAdd);
        }

        private void _addExpenseCompleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] = null;
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
                        MessageBox.Show("Unable to add expense", "Error", MessageBoxButton.OK);
                    }
                });
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}