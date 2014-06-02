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

namespace Split_It_.Add_Expense_Pages
{
    public partial class EditExpense : PhoneApplicationPage
    {
        BackgroundWorker editExpenseBackgroundWorker;
        ApplicationBarIconButton btnOkay;

        public EditExpense()
        {
            InitializeComponent();

            editExpenseBackgroundWorker = new BackgroundWorker();
            editExpenseBackgroundWorker.WorkerSupportsCancellation = true;
            editExpenseBackgroundWorker.DoWork += new DoWorkEventHandler(editExpenseBackgroundWorker_DoWork);

            createAppBar();

            this.expenseControl.tbDescription.TextChanged += tbDescription_TextChanged;
            this.expenseControl.tbAmount.TextChanged += tbAmount_TextChanged;

            setupViews();

            //This has to be setup after we display the expense's values so that these dont override their values
            this.expenseControl.groupListPicker.SelectionChanged += groupListPicker_SelectionChanged;
            this.expenseControl.friendListPicker.SelectionChanged += friendListPicker_SelectionChanged;
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
            btnOkay.IconUri = new Uri("/Assets/Icons/ok.png", UriKind.Relative);
            btnOkay.Text = "ok";
            btnOkay.IsEnabled = false;
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
            
            this.expenseControl.amountSplit = ExpenseUserControl.AmountSplit.Split_equally;

            this.expenseControl.groupListPicker.SelectedItem = getSelectedGroup();
            setupSelectedUsers();
        }

        private Group getSelectedGroup()
        {
            if (this.expenseControl.expense.group_id == 0)
                return null;
            else
            {
                foreach (var group in App.groupsList)
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
                    this.expenseControl.expenseShareUsers.Add(expenseUser);
                    if (expenseUser.user.id != App.currentUser.id)
                        this.expenseControl.friendListPicker.SelectedItems.Add(expenseUser.user);
                }
            }
        }
        
        private void btnOk_Click(object sender, EventArgs e)
        {
            //FocusedTextBoxUpdateSource();
            //to hide the keyboard if any
            this.Focus();
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
            this.expenseControl.expense.group_id = 0;
            if (this.expenseControl.groupListPicker.SelectedItem == null)
                return;

            if (validGroupSelected())
            {
                Group selectedGroup = this.expenseControl.groupListPicker.SelectedItem as Group;
                this.expenseControl.expense.group_id = selectedGroup.id;
                foreach (var member in selectedGroup.members)
                {
                    //you don't need to add yourself as you will be added by default.
                    if (member.id == App.currentUser.id || this.expenseControl.friendListPicker.SelectedItems.Contains(member))
                        continue;
                    this.expenseControl.friendListPicker.SelectedItems.Add(member);
                }
            }

            else
            {
                MessageBox.Show("Sorry you can only select one group", "Error", MessageBoxButton.OK);
                //this.friendListPicker.SelectedItems.Clear();
                this.expenseControl.groupListPicker.SelectedItems.Clear();
            }
        }

        private void friendListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.expenseControl.expenseShareUsers.Clear();
            //add yourself to the the list of expense users.
            this.expenseControl.expenseShareUsers.Add(new Expense_Share() { user = App.currentUser, user_id = App.currentUser.id });
            enableOkButton();

            if (this.expenseControl.friendListPicker.SelectedItems == null)
                return;
            foreach (var item in this.expenseControl.friendListPicker.SelectedItems)
            {
                User user = item as User;
                this.expenseControl.expenseShareUsers.Add(new Expense_Share() { user = user, user_id = user.id });
            }
        }
        
        protected void tbDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.expenseControl.expense.description = this.expenseControl.tbDescription.Text;
            enableOkButton();
        }

        protected void tbAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.expenseControl.expense.cost = this.expenseControl.tbAmount.Text;
            enableOkButton();
        }

        private void enableOkButton()
        {
            if (this.expenseControl.friendListPicker.SelectedItems != null && !String.IsNullOrEmpty(this.expenseControl.tbAmount.Text) && !String.IsNullOrEmpty(this.expenseControl.tbDescription.Text))
            {
                btnOkay.IsEnabled = true;
            }
            else
                btnOkay.IsEnabled = false;
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
    }
}