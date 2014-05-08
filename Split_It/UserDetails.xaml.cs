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
using System.Windows.Media;

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
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;
            ApplicationBar.BackgroundColor = (Color) Application.Current.Resources["green"];
            ApplicationBar.ForegroundColor = Colors.White;

            //Settle up button
            ApplicationBarIconButton btnSettle = new ApplicationBarIconButton();
            btnSettle.IconUri = new Uri("/Assets/Icons/feature.email.png", UriKind.Relative);
            btnSettle.Text = "reminder";
            Balance_User defaultBalance = Util.getDefaultBalance(selectedUser.balance);
            double finalBalance = System.Convert.ToDouble(defaultBalance.amount);
            if (finalBalance <= 0)
                btnSettle.IsEnabled = false;
            ApplicationBar.Buttons.Add(btnSettle);
            btnSettle.Click += new EventHandler(btnSettle_Click);
        }

        private void btnSettle_Click(object sender, EventArgs e)
        {
            
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