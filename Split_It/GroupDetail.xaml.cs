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
using System.ComponentModel;
using System.Collections.ObjectModel;
using Split_It_.Utils;
using Split_It_.Controller;

namespace Split_It_
{
    public partial class GroupDetail : PhoneApplicationPage
    {
        Group selectedGroup;
        BackgroundWorker groupExpensesBackgroundWorker;
        ObservableCollection<Expense> expensesList = new ObservableCollection<Expense>();
        private int pageNo = 0;
        private bool morePages = true;
        private object o = new object();

        public GroupDetail()
        {
            InitializeComponent();

            selectedGroup = PhoneApplicationService.Current.State[Constants.SELECTED_GROUP] as Group;
            llsExpenses.ItemsSource = expensesList;

            groupExpensesBackgroundWorker = new BackgroundWorker();
            groupExpensesBackgroundWorker.WorkerSupportsCancellation = true;
            groupExpensesBackgroundWorker.DoWork += new DoWorkEventHandler(groupExpensesBackgroundWorker_DoWork);

            if (groupExpensesBackgroundWorker.IsBusy != true)
            {
                groupExpensesBackgroundWorker.RunWorkerAsync();
            }

            //createAppBar();
            this.DataContext = selectedGroup;
        }

        private void groupExpensesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadExpenses();
        }

        private void loadExpenses()
        {
            //the rest of the work is done in a backgroundworker
            QueryDatabase obj = new QueryDatabase();
            List<Expense> allExpenses = obj.getAllExpensesForGroup(selectedGroup.id, pageNo);

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

                    if (groupExpensesBackgroundWorker.IsBusy != true && morePages && expensesList.Count - expensesList.IndexOf(expense) <= offset)
                    {
                        pageNo++;
                        groupExpensesBackgroundWorker.RunWorkerAsync();
                    }
                }
            }
        }

        private void llsExpenses_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            Expense selectedExpense = selector.SelectedItem as Expense;

            if (selectedExpense == null)
                return;

            PhoneApplicationService.Current.State[Constants.SELECTED_EXPENSE] = selectedExpense;
            NavigationService.Navigate(new Uri("/ExpenseDetail.xaml", UriKind.Relative));
        }
    }
}