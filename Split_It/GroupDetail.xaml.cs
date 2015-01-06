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
using Telerik.Windows.Controls;
using Split_It_.UserControls;

namespace Split_It_
{
    public partial class GroupDetail : PhoneApplicationPage
    {
        Group selectedGroup;
        BackgroundWorker groupExpensesBackgroundWorker;
        ObservableCollection<Expense> expensesList = new ObservableCollection<Expense>();
        ObservableCollection<ExpandableListModel> expanderList = new ObservableCollection<ExpandableListModel>();
        ExpandableListModel currentUserExpanderInfo;
        private int pageNo = 0;
        private bool morePages = true;
        private object o = new object();

        //Popups
        Telerik.Windows.Controls.RadWindow SettleUpWindow;

        public GroupDetail()
        {
            InitializeComponent();

            selectedGroup = PhoneApplicationService.Current.State[Constants.SELECTED_GROUP] as Group;
            llsExpenses.ItemsSource = expensesList;
            this.llsExpenses.DataRequested += this.OnDataRequested;

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
                //if(selectedGroup.simplify_by_default)
                    expanderItem.debtList = Util.getUsersGroupDebtsList(selectedGroup.simplified_debts, user.id);
                //else
                   // expanderItem.debtList = Util.getUsersGroupDebtsList(selectedGroup.original_debts, user.id);

                if (expanderItem.debtList.Count == 0)
                    expanderItem.isNonExpandable = true;

                expanderList.Add(expanderItem);

                if (user.id == App.currentUser.id)
                    currentUserExpanderInfo = expanderItem;
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

            //Add expense to group
            ApplicationBarIconButton btnAddExpense = new ApplicationBarIconButton();
            btnAddExpense.IconUri = new Uri("/Assets/Icons/add.png", UriKind.Relative);
            btnAddExpense.Text = "add";
            ApplicationBar.Buttons.Add(btnAddExpense);
            btnAddExpense.Click += new EventHandler(btnAddExpense_Click);

            //Settle up button
            ApplicationBarIconButton btnSettle = new ApplicationBarIconButton();
            btnSettle.IconUri = new Uri("/Assets/Icons/settle.png", UriKind.Relative);
            btnSettle.Text = "settle";
            btnSettle.IsEnabled = false;

            if (currentUserExpanderInfo!=null && !currentUserExpanderInfo.isNonExpandable)
                btnSettle.IsEnabled = true;

            ApplicationBar.Buttons.Add(btnSettle);
            btnSettle.Click += new EventHandler(btnSettle_Click);
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

        private void OnDataRequested(object sender, EventArgs e)
        {
            lock (o)
            {
                if (groupExpensesBackgroundWorker.IsBusy != true && morePages)
                {
                    pageNo++;
                    groupExpensesBackgroundWorker.RunWorkerAsync(false);
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

        private void btnSettle_Click(object sender, EventArgs e)
        {
            if (currentUserExpanderInfo.debtList.Count == 1)
                recordPayment(currentUserExpanderInfo.debtList[0]);
            else
            {
                SettleUpWindow = new Telerik.Windows.Controls.RadWindow();
                GroupSettleUpUserSelector ChoosePayeePopup = new GroupSettleUpUserSelector(currentUserExpanderInfo.debtList, _SettleUpSelectorClose);
                ChoosePayeePopup.MaxHeight = App.Current.Host.Content.ActualHeight / 1.1;
                ChoosePayeePopup.MaxWidth = App.Current.Host.Content.ActualWidth / 1.1;

                SettleUpWindow.Content = ChoosePayeePopup;
                ShowRadWindow(ref SettleUpWindow);
            }
        }

        private void _SettleUpSelectorClose(Debt_Group debt)
        {
            SettleUpWindow.IsOpen = false;
            recordPayment(debt);
        }

        private void ShowRadWindow(ref RadWindow window)
        {
            window.Placement = Telerik.Windows.Controls.PlacementMode.CenterCenter;
            window.IsOpen = true;
        }

        private void recordPayment(Debt_Group debt)
        {
            User user;
            String amount;
            if (debt.ownerId == debt.from)
            {
                PhoneApplicationService.Current.State[Constants.PAYMENT_TYPE] = Constants.PAYMENT_TO;
                user = debt.toUser;

                //the amount for group debts is always in +ve but in payment page, we need it in correct +/- format
                amount = "-" + debt.amount;
            }
            else
            {
                PhoneApplicationService.Current.State[Constants.PAYMENT_TYPE] = Constants.PAYMENT_FROM;
                user = debt.fromUser;
                amount = debt.amount;
            }
            if (user.balance == null)
                user.balance = new List<Balance_User>();

            user.balance.Add(new Balance_User() { amount = amount, currency_code = debt.currency_code, user_id = user.id });
            PhoneApplicationService.Current.State[Constants.PAYMENT_USER] = user;
            PhoneApplicationService.Current.State[Constants.PAYMENT_GROUP] = selectedGroup.id;

            NavigationService.Navigate(new Uri("/Add_Expense_Pages/AddPayment.xaml", UriKind.Relative));
        }
    }
}