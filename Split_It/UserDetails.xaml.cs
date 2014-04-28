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
using System.ComponentModel;
using Split_It_.Controller;
using System.Collections.ObjectModel;

namespace Split_It_
{
    public partial class UserDetails : PhoneApplicationPage
    {
        int selectedUserId;
        BackgroundWorker userExpensesBackgroundWorker;
        ObservableCollection<Expense> expensesList = new ObservableCollection<Expense>();

        public UserDetails()
        {
            InitializeComponent();

            //selectedUser = PhoneApplicationService.Current.State[Constants.SELECTED_USER] as User;

            userExpensesBackgroundWorker = new BackgroundWorker();
            userExpensesBackgroundWorker.WorkerSupportsCancellation = true;
            userExpensesBackgroundWorker.DoWork += new DoWorkEventHandler(userExpensesBackgroundWorker_DoWork);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string userId;
            //This condition will only be true if the user has launched this page. This paramter (afterLogin) wont be there
            //if the page has been accessed from the back stack
            if (NavigationContext.QueryString.TryGetValue("selectedUserId", out userId))
            {
                if (userId!=null)
                {
                    selectedUserId = Convert.ToInt32(userId);
                    if (userExpensesBackgroundWorker.IsBusy != true)
                    {
                        userExpensesBackgroundWorker.RunWorkerAsync();
                    }
                }
            }
        }

        private void userExpensesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadExpenses();
        }

        private void loadExpenses()
        {
            //the rest of the work is done in a backgroundworker
            QueryDatabase obj = new QueryDatabase();
            List<Expense> allExpenses = obj.getExpensesForUser(selectedUserId);

            Dispatcher.BeginInvoke(() =>
            {
                expensesList.Clear();
                foreach (var expense in allExpenses)
                {
                    expensesList.Add(expense);
                }
            });

            obj.closeDatabaseConnection();
        }
    }
}