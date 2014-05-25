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
using System.ComponentModel;
using Split_It_.Controller;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Microsoft.Phone.Tasks;

namespace Split_It_
{
    public partial class UserDetails : PhoneApplicationPage
    {
        User selectedUser;
        BackgroundWorker userExpensesBackgroundWorker;
        ObservableCollection<Expense> expensesList = new ObservableCollection<Expense>();
        private int pageNo = 0;
        private bool morePages = true;
        private object o = new object();

        public UserDetails()
        {
            InitializeComponent();

            selectedUser = PhoneApplicationService.Current.State[Constants.SELECTED_USER] as User;

            llsExpenses.ItemsSource = expensesList;
            
            userExpensesBackgroundWorker = new BackgroundWorker();
            userExpensesBackgroundWorker.WorkerSupportsCancellation = true;
            userExpensesBackgroundWorker.DoWork += new DoWorkEventHandler(userExpensesBackgroundWorker_DoWork);

            if (userExpensesBackgroundWorker.IsBusy != true)
            {
                userExpensesBackgroundWorker.RunWorkerAsync();
            }

            createAppBar();
            this.DataContext = selectedUser;
        }

        private void createAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;
            ApplicationBar.BackgroundColor = (Color) Application.Current.Resources["green"];
            ApplicationBar.ForegroundColor = Colors.White;

            //Settle up button
            ApplicationBarIconButton btnReminder = new ApplicationBarIconButton();
            btnReminder.IconUri = new Uri("/Assets/Icons/feature.email.png", UriKind.Relative);
            btnReminder.Text = "reminder";

            ApplicationBarIconButton btnSettle = new ApplicationBarIconButton();
            btnSettle.IconUri = new Uri("/Assets/Icons/settle.png", UriKind.Relative);
            btnSettle.Text = "settle";


            Balance_User defaultBalance = Util.getDefaultBalance(selectedUser.balance);
            double finalBalance = System.Convert.ToDouble(defaultBalance.amount);
            if (finalBalance <= 0)
            {
                btnReminder.IsEnabled = false;
                btnSettle.IsEnabled = true;
            }
            else
            {
                btnReminder.IsEnabled = true;
                btnSettle.IsEnabled = false;
            }

            ApplicationBar.Buttons.Add(btnReminder);
            ApplicationBar.Buttons.Add(btnSettle);
            btnReminder.Click += new EventHandler(btnReminder_Click);
            btnSettle.Click += new EventHandler(btnSettle_Click);
        }

        private void btnSettle_Click(object sender, EventArgs e)
        {
            PhoneApplicationService.Current.State[Constants.PAYMENT_TO_USER] = selectedUser;
            NavigationService.Navigate(new Uri("/Add_Expense_Pages/AddPayment.xaml", UriKind.Relative));
        }

        private void btnReminder_Click(object sender, EventArgs e)
        {
            string appUrl = "";
            string reminderText = "Hey there,\n\nThis is just a note to settle up on Splitwise as soon as you get the chance.\n\n";
            string thanks = "Thanks,\n";
            string userName = App.currentUser.first_name;
            string sentVia = "\n\nSent via,\n";
            string appName = "Split it! A splitwise client for windows phone\n\n";
            string downloadApp = "Download app at: " + appUrl;

            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "Settle up on Splitwise";
            emailComposeTask.Body = reminderText + thanks + userName + sentVia + appName + downloadApp;
            emailComposeTask.To = selectedUser.email;

            emailComposeTask.Show();
        }

        private void userExpensesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadExpenses();
        }

        private void loadExpenses()
        {
            //the rest of the work is done in a backgroundworker
            QueryDatabase obj = new QueryDatabase();
            List<Expense> allExpenses = obj.getExpensesForUser(selectedUser.id, pageNo);

            Dispatcher.BeginInvoke(() =>
            {
                if (allExpenses == null || allExpenses.Count == 0)
                    morePages = false;

                foreach (var expense in allExpenses)
                {
                    expensesList.Add(expense);
                }
            });

            obj.closeDatabaseConnection();
        }

        private void llsExpenses_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            lock (o)
            {
                Expense expense = e.Container.Content as Expense;
                if (expense != null)
                {
                    int offset = 2;

                    if (userExpensesBackgroundWorker.IsBusy != true && morePages && expensesList.Count - expensesList.IndexOf(expense) <= offset)
                    {
                        pageNo++;
                        userExpensesBackgroundWorker.RunWorkerAsync();
                    }
                }
            }
        }

        private void llsExpenses_Tap(object sender, SelectionChangedEventArgs e)
        {
            if (llsExpenses.SelectedItem == null)
                return;
            //LongListSelector selector = sender as LongListSelector;
            Expense selectedExpense = llsExpenses.SelectedItem as Expense;

            //if (selectedExpense == null)
            //return;

            PhoneApplicationService.Current.State[Constants.SELECTED_EXPENSE] = selectedExpense;
            NavigationService.Navigate(new Uri("/ExpenseDetail.xaml", UriKind.Relative));

            llsExpenses.SelectedItem = null;
        }
    }
}