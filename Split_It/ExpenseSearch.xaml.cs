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
using System.Collections.ObjectModel;
using Split_It_.Utils;
using Telerik.Windows.Controls;
using System.ComponentModel;
using Split_It_.Controller;

namespace Split_It_
{
    public partial class ExpenseSearch : PhoneApplicationPage
    {
        ObservableCollection<Expense> expenseSearchResult = new ObservableCollection<Expense>();
        BackgroundWorker searchExpenseBackgroundWorker;

        public ExpenseSearch()
        {
            InitializeComponent();

            searchExpenseBackgroundWorker = new BackgroundWorker();
            searchExpenseBackgroundWorker.WorkerSupportsCancellation = true;
            searchExpenseBackgroundWorker.DoWork += new DoWorkEventHandler(searchExpenseBackgroundWorker_DoWork);

            llsExpenses.ItemsSource = expenseSearchResult;
        }

        private void llsExpenses_Tap(object sender, SelectionChangedEventArgs e)
        {
            if (llsExpenses.SelectedItem == null)
                return;
            Expense selectedExpense = llsExpenses.SelectedItem as Expense;

            PhoneApplicationService.Current.State[Constants.SELECTED_EXPENSE] = selectedExpense;
            NavigationService.Navigate(new Uri("/ExpenseDetail.xaml", UriKind.Relative));

            llsExpenses.SelectedItem = null;
        }

        private void tbSearch_ActionButtonTap(object sender, EventArgs e)
        {
            string searchText = tbSearch.Text;
            if(!String.IsNullOrEmpty(searchText))
                search(searchText);
        }

        private void search(string text)
        {
            this.Focus();
            busyIndicator.IsRunning = true;
            expenseSearchResult.Clear();
            searchExpenseBackgroundWorker.RunWorkerAsync(text);
        }

        private void tbSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                string searchText = tbSearch.Text;
                if (!String.IsNullOrEmpty(searchText))
                    search(searchText);
            }
        }

        private void searchExpenseBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            QueryDatabase obj = new QueryDatabase();
            List<Expense> allExpenses = obj.searchForExpense(e.Argument.ToString());
            Dispatcher.BeginInvoke(() =>
            {
                if (allExpenses != null)
                {
                    foreach (var expense in allExpenses)
                    {
                        expenseSearchResult.Add(expense);
                    }
                }

                busyIndicator.IsRunning = false;
            });
            obj.closeDatabaseConnection();
        }
    }
}