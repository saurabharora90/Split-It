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
using System.ComponentModel;
using Split_It_.Controller;
using System.Windows.Media;
using Split_It_.Model;
using Split_It_.UserControls;

namespace Split_It_.Add_Expense_Pages
{
    public partial class EditExpense : PhoneApplicationPage
    {
        BackgroundWorker editExpenseBackgroundWorker;
        ApplicationBarIconButton btnOkay;

        bool groupSelectionFirstTime = true;

        public EditExpense()
        {
            InitializeComponent();

            editExpenseBackgroundWorker = new BackgroundWorker();
            editExpenseBackgroundWorker.WorkerSupportsCancellation = true;
            editExpenseBackgroundWorker.DoWork += new DoWorkEventHandler(editExpenseBackgroundWorker_DoWork);

            createAppBar();
            this.expenseControl.setDimBackGround(DimBackGround);
            setupViews();
            this.expenseControl.SetupListeners();
            this.expenseControl.groupListPicker.SelectionChanged += groupListPicker_SelectionChanged;

            //setup the expenseShareUsers from the actual expense.
            this.expenseControl.expenseShareUsers.Clear();
            foreach (var item in this.expenseControl.expense.users)
            {
                this.expenseControl.expenseShareUsers.Add(item);
            }
        }

        protected void createAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;
            ApplicationBar.BackgroundColor = (Color)Application.Current.Resources["green"];
            ApplicationBar.ForegroundColor = Colors.White;

            btnOkay = new ApplicationBarIconButton();
            btnOkay.IconUri = new Uri("/Assets/Icons/save.png", UriKind.Relative);
            btnOkay.Text = "save";
            ApplicationBar.Buttons.Add(btnOkay);
            btnOkay.Click += new EventHandler(btnOk_Click);

            ApplicationBarIconButton btnCancel = new ApplicationBarIconButton();
            btnCancel.IconUri = new Uri("/Assets/Icons/cancel.png", UriKind.Relative);
            btnCancel.Text = "cancel";
            ApplicationBar.Buttons.Add(btnCancel);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        private void setupViews()
        {
            this.expenseControl.tbDescription.Text = this.expenseControl.expense.description;
            this.expenseControl.tbAmount.Text = this.expenseControl.expense.cost;

            if (!String.IsNullOrEmpty(this.expenseControl.expense.details) && !this.expenseControl.expense.details.Equals(Expense.DEFAULT_DETAILS))
            {
                this.expenseControl.tbDetails.Text = this.expenseControl.expense.details;
            }
            this.expenseControl.expenseDate.Value = DateTime.Parse(this.expenseControl.expense.date, System.Globalization.CultureInfo.InvariantCulture);
            this.expenseControl.groupListPicker.SelectedItem = getSelectedGroup();
            setupSelectedUsers();

            //setup the payee
            int payeeCount = 0;
            Expense_Share payee = null;
            foreach (var item in this.expenseControl.expense.users)
            {
                if (item.hasPaid)
                {
                    payee = item;
                    payeeCount++;
                }
            }

            if (payeeCount > 1)
            {
                this.expenseControl.tbPaidBy.Text = "Multiple users";
                this.expenseControl.PaidByUser = null;
            }
            else
            {
                this.expenseControl.PaidByUser = payee;
                this.expenseControl.tbPaidBy.Text = payee.ToString();
            }

            //setup the expense to be split unequally for ease of operation
            this.expenseControl.SplitTypeListPicker.SelectedItem = AmountSplit.UnequalSplit;
        }

        private Group getSelectedGroup()
        {
            //not assocated to any expense. therefore the groupSelectionChanged will not be fired.
            if (this.expenseControl.expense.group_id == 0)
            {
                groupSelectionFirstTime = false;
                return null;
            }
            else
            {
                foreach (var group in expenseControl.groupsList)
                {
                    if (this.expenseControl.expense.group_id == group.id)
                        return group;
                }
            }

            return null;
        }

        private void setupSelectedUsers()
        {
            if (this.expenseControl.expense.users.Count == 0)
                return;
            else
            {
                foreach (var expenseUser in this.expenseControl.expense.users)
                {
                    //this.expenseControl.expenseShareUsers.Add(expenseUser);
                    if (expenseUser.user.id != App.currentUser.id)
                        this.expenseControl.friendListPicker.SelectedItems.Add(expenseUser);
                }
            }
        }
        
        private void btnOk_Click(object sender, EventArgs e)
        {
            bool proceed = this.expenseControl.setupExpense();
            if (editExpenseBackgroundWorker.IsBusy != true)
            {
                busyIndicator.IsRunning = true;

                if (proceed)
                    editExpenseBackgroundWorker.RunWorkerAsync();
                else
                    busyIndicator.IsRunning = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] = null;
            NavigationService.GoBack();
        }

        //returns true only if one or less group is selected
        private bool validGroupSelected()
        {
            if (this.expenseControl.groupListPicker.SelectedItems.Count > 1)
                return false;
            else
                return true;
        }

        private void groupListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //not to use the shortcut when we are filling up the details of the expense to be edited
            if (groupSelectionFirstTime)
            {
                groupSelectionFirstTime = false;
                return;
            }
            this.expenseControl.expense.group_id = 0;
            if (this.expenseControl.groupListPicker.SelectedItem == null)
                return;

            if (validGroupSelected())
            {
                Group selectedGroup = this.expenseControl.groupListPicker.SelectedItem as Group;
                this.expenseControl.expense.group_id = selectedGroup.id;

                //clear all the previoulsy selected friends.
                this.expenseControl.friendListPicker.SelectedItems.Clear();

                foreach (var member in selectedGroup.members)
                {
                    //you don't need to add yourself as you will be added by default.
                    if (member.id == App.currentUser.id || this.expenseControl.friendListPicker.SelectedItems.Contains(member))
                        continue;
                    this.expenseControl.friendListPicker.SelectedItems.Add(new Expense_Share() { user = member, user_id = member.id });
                }
            }

            else
            {
                MessageBox.Show("Sorry you can only select one group", "Error", MessageBoxButton.OK);
                //this.friendListPicker.SelectedItems.Clear();
                this.expenseControl.groupListPicker.SelectedItems.Clear();
            }
        }
        
        private void editExpenseBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ModifyDatabase modify = new ModifyDatabase(_editExpenseCompleted);
            modify.editExpense(this.expenseControl.expense);
        }

        private void _editExpenseCompleted(bool success, HttpStatusCode errorCode)
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
                        MessageBox.Show("Unable to edit expense", "Error", MessageBoxButton.OK);
                    }
                });
            }
        }

        private void DimBackGround(bool dim)
        {
            if (dim)
            {
                DimContainer.Visibility = System.Windows.Visibility.Visible;
                ApplicationBar.IsVisible = false;
            }
            else
            {
                DimContainer.Visibility = System.Windows.Visibility.Collapsed;
                ApplicationBar.IsVisible = true;
            }
        }
    }
}