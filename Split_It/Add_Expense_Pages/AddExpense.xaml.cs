using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections;
using Split_It_.Model;
using System.Windows.Media;
using Split_It_.Utils;
using System.ComponentModel;
using Split_It_.Controller;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Split_It_.UserControls;

namespace Split_It_.Add_Expense_Pages
{
    public partial class AddExpense : PhoneApplicationPage
    {
        BackgroundWorker addExpenseBackgroundWorker;
        ApplicationBarIconButton btnOkay;
        
        public AddExpense()
        {
            InitializeComponent();

            addExpenseBackgroundWorker = new BackgroundWorker();
            addExpenseBackgroundWorker.WorkerSupportsCancellation = true;
            addExpenseBackgroundWorker.DoWork += new DoWorkEventHandler(addExpenseBackgroundWorker_DoWork);

            createAppBar();

            this.expenseControl.amountSplit = ExpenseUserControl.AmountSplit.Split_equally;

            this.expenseControl.groupListPicker.SelectionChanged += groupListPicker_SelectionChanged;

            // By default, the current user is set as the payee of the expense.
            Expense_Share currentUser = new Expense_Share() { user = App.currentUser, user_id = App.currentUser.id };
            this.expenseControl.paidByListPicker.SelectedItem = currentUser;

            this.expenseControl.tbDescription.TextChanged += tbDescription_TextChanged;
            this.expenseControl.tbAmount.TextChanged += tbAmount_TextChanged;

            this.expenseControl.groupListPicker.SelectedItem = getFromGroup();
            this.expenseControl.friendListPicker.SelectedItem = getFromFriend();
            
            this.expenseControl.expenseDate.Value = DateTime.Now;
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
            btnOkay.IsEnabled = false;
            ApplicationBar.Buttons.Add(btnOkay);
            btnOkay.Click += new EventHandler(btnOk_Click);

            ApplicationBarIconButton btnCancel = new ApplicationBarIconButton();
            btnCancel.IconUri = new Uri("/Assets/Icons/cancel.png", UriKind.Relative);
            btnCancel.Text = "cancel";
            ApplicationBar.Buttons.Add(btnCancel);
            btnCancel.Click += new EventHandler(btnCancel_Click);

            ApplicationBarIconButton btnPin = new ApplicationBarIconButton();
            btnPin.IconUri = new Uri("/Assets/Icons/pin.png", UriKind.Relative);
            btnPin.Text = "pin";
            ApplicationBar.Buttons.Add(btnPin);
            btnPin.Click += new EventHandler(btnPin_Click);
            ShellTile tile = Util.FindTile(Constants.ADD_EXPENSE_TILE_SHORTCUT);
            if (tile != null)
                btnPin.IsEnabled = false;
        }

        private Group getFromGroup()
        {
            if (this.expenseControl.expense == null)
                return null;
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

        private Expense_Share getFromFriend()
        {
            if (this.expenseControl.expense == null)
                return null;
            else
            {
                foreach (var user in expenseControl.expenseShareUsers)
                {
                    if (this.expenseControl.expense.specificUserId == user.user_id)
                        return user;
                }
            }

            return null;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            bool proceed = this.expenseControl.setupExpense();
            if (addExpenseBackgroundWorker.IsBusy != true)
            {
                busyIndicator.Content = "adding expense";
                busyIndicator.IsRunning = true;

                if (proceed)
                    addExpenseBackgroundWorker.RunWorkerAsync();
                else
                    busyIndicator.IsRunning = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] = null;

            //This page might be accessed from the start screen tile and hence canGoBack might be false
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                Application.Current.Terminate();
        }

        private void btnPin_Click(object sender, EventArgs e)
        {
            StandardTileData tileData = new StandardTileData
            {
                Title = "add expense",
                BackgroundImage = new Uri("/Assets/Tiles/TileMediumAdd.png", UriKind.Relative),
            };

            Uri tileUri = new Uri(Constants.ADD_EXPENSE_TILE_SHORTCUT, UriKind.Relative);
            ShellTile.Create(tileUri, tileData);
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

                //clear all the previoulsy selected friends.
                this.expenseControl.friendListPicker.SelectedItems.Clear();
                
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
            if (this.expenseControl.friendListPicker.SelectedItems.Count!=0 && !String.IsNullOrEmpty(this.expenseControl.tbAmount.Text) && !String.IsNullOrEmpty(this.expenseControl.tbDescription.Text))
            {
                btnOkay.IsEnabled = true;
            }
            else
                btnOkay.IsEnabled = false;
        }

        private void addExpenseBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ModifyDatabase modify = new ModifyDatabase(_addExpenseCompleted);
            modify.addExpense(this.expenseControl.expense);
        }

        private void _addExpenseCompleted(bool success, HttpStatusCode errorCode)
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
                        MessageBox.Show("Unable to add expense", "Error", MessageBoxButton.OK);
                    }
                });
            }
        }
    }
}