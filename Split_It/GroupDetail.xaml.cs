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
using Split_It_.ExpandableListHelper;
using System.Windows.Media;

namespace Split_It_
{
    public partial class GroupDetail : PhoneApplicationPage
    {
        Group selectedGroup;
        BackgroundWorker groupExpensesBackgroundWorker;
        ObservableCollection<Expense> expensesList = new ObservableCollection<Expense>();
        ObservableCollection<ExpandableListModel> expanderList = new ObservableCollection<ExpandableListModel>();
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

            setupExpandableList();
            createAppBar();
            this.DataContext = selectedGroup;
            listBox.ItemsSource = expanderList;
        }

        private void setupExpandableList()
        {
            foreach (var user in selectedGroup.members)
            {
                ExpandableListModel expanderItem = new ExpandableListModel();
                expanderItem.groupUser = user;
                if(selectedGroup.simplify_by_default)
                    expanderItem.debtList = Util.getUsersGroupDebtsList(selectedGroup.simplified_debts, user.id);
                else
                    expanderItem.debtList = Util.getUsersGroupDebtsList(selectedGroup.original_debts, user.id);

                if (expanderItem.debtList.Count == 0)
                    expanderItem.isNonExpandable = true;

                expanderList.Add(expanderItem);
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
            ApplicationBarIconButton btnAddExpense = new ApplicationBarIconButton();
            btnAddExpense.IconUri = new Uri("/Assets/Icons/add.png", UriKind.Relative);
            btnAddExpense.Text = "add";
            ApplicationBar.Buttons.Add(btnAddExpense);
            btnAddExpense.Click += new EventHandler(btnAddExpense_Click);
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

        private void llsExpenses_Tap(object sender, SelectionChangedEventArgs e)
        {
            if (llsExpenses.SelectedItem == null)
                return;
            Expense selectedExpense = llsExpenses.SelectedItem as Expense;

            PhoneApplicationService.Current.State[Constants.SELECTED_EXPENSE] = selectedExpense;
            NavigationService.Navigate(new Uri("/ExpenseDetail.xaml", UriKind.Relative));

            llsExpenses.SelectedItem = null;
        }

        private void btnAddExpense_Click(object sender, EventArgs e)
        {
            Expense expenseToAdd = new Expense();
            expenseToAdd.group_id = selectedGroup.id;

            PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] = expenseToAdd;
            NavigationService.Navigate(new Uri("/Add_Expense_Pages/AddExpense.xaml", UriKind.Relative));
        }
    }
}