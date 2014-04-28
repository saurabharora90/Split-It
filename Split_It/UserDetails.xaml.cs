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

            //setUIDetails();
            this.DataContext = selectedUser;
        }

        private void setUIDetails()
        {
            //tbTile.Text = selectedUser.first_name;
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
                if (userExpensesBackgroundWorker.IsBusy != true && morePages)
                {
                    pageNo++;
                    userExpensesBackgroundWorker.RunWorkerAsync();
                }
            }
        }
    }
}