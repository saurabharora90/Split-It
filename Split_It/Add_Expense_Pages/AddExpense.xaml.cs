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

            this.expenseControl.amountSplit = AmountSplit.EqualSplit;
            this.expenseControl.setDimBackGround(DimBackGround);
            this.expenseControl.groupListPicker.SelectionChanged += groupListPicker_SelectionChanged;

            //This helps to auto-populate if the user is coming from the GroupDetails or UserDetails page
            autoPopulateGroup();
            autoPopulateExpenseShareUsers();
            
            this.expenseControl.expenseDate.Value = DateTime.Now;
            this.expenseControl.SetupListeners();
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

            ApplicationBarIconButton btnPin = new ApplicationBarIconButton();
            btnPin.IconUri = new Uri("/Assets/Icons/pin.png", UriKind.Relative);
            btnPin.Text = "pin";
            ApplicationBar.Buttons.Add(btnPin);
            btnPin.Click += new EventHandler(btnPin_Click);
            ShellTile tile = Util.FindTile(Constants.ADD_EXPENSE_TILE_SHORTCUT);
            if (tile != null)
                btnPin.IsEnabled = false;
        }

        private void autoPopulateGroup()
        {
            if (this.expenseControl.expense == null)
                return;
            else
            {
                foreach (var group in expenseControl.groupsList)
                {
                    if (this.expenseControl.expense.group_id == group.id)
                        this.expenseControl.groupListPicker.SelectedItem = group;
                }
            }
        }

        private void autoPopulateExpenseShareUsers()
        {
            if (this.expenseControl.expense == null)
                return;
            else
            {
                foreach (var user in expenseControl.friends)
                {
                    if (this.expenseControl.expense.specificUserId == user.user_id)
                        this.expenseControl.friendListPicker.SelectedItems.Add(user);
                }
            }
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