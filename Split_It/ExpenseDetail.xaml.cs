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
using System.Windows.Media;
using Split_It_.Controller;
using System.ComponentModel;

namespace Split_It_
{
    public partial class ExpenseDetail : PhoneApplicationPage
    {
        Expense selectedExpense;
        BackgroundWorker deleteExpenseBackgroundWorker;

        public ExpenseDetail()
        {
            InitializeComponent();

            selectedExpense = PhoneApplicationService.Current.State[Constants.SELECTED_EXPENSE] as Expense;
            selectedExpense.displayType = Expense.DISPLAY_FOR_ALL_USER;
            this.DataContext = selectedExpense;
            llsRepayments.ItemsSource = selectedExpense.users;
            createAppBar();

            deleteExpenseBackgroundWorker = new BackgroundWorker();
            deleteExpenseBackgroundWorker.WorkerSupportsCancellation = true;
            deleteExpenseBackgroundWorker.DoWork += new DoWorkEventHandler(deleteExpenseBackgroundWorker_DoWork);
        }

        private void createAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;
            ApplicationBar.BackgroundColor = (Color)Application.Current.Resources["green"];
            ApplicationBar.ForegroundColor = Colors.White;

            //delete expense button
            ApplicationBarIconButton btnDelete = new ApplicationBarIconButton();
            btnDelete.IconUri = new Uri("/Assets/Icons/delete.png", UriKind.Relative);
            btnDelete.Text = "delete";
            ApplicationBar.Buttons.Add(btnDelete);
            btnDelete.Click += new EventHandler(btnDelete_Click);

            //edit expense button
            ApplicationBarIconButton btnEdit = new ApplicationBarIconButton();
            btnEdit.IconUri = new Uri("/Assets/Icons/edit.png", UriKind.Relative);
            btnEdit.Text = "edit";
            ApplicationBar.Buttons.Add(btnEdit);
            btnEdit.Click += new EventHandler(btnEdit_Click);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = "Delete Expense",
                Message = "Are you sure?",
                LeftButtonContent = "yes",
                RightButtonContent = "no",
                IsFullScreen = false
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        if (deleteExpenseBackgroundWorker.IsBusy != true)
                        {
                            SystemTray.ProgressIndicator = new ProgressIndicator();
                            SystemTray.ProgressIndicator.IsIndeterminate = true;
                            SystemTray.ProgressIndicator.IsVisible = true;

                            deleteExpenseBackgroundWorker.RunWorkerAsync();
                        }
                        break;
                    case CustomMessageBoxResult.RightButton:
                        // Do something.
                        break;
                    case CustomMessageBoxResult.None:
                        // Do something.
                        break;
                    default:
                        break;
                }
            };

            messageBox.Show();
        }

        private void deleteExpenseBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ModifyDatabase modify = new ModifyDatabase(_deleteExpenseCompleted);
            modify.deleteExpense(selectedExpense.id);
        }

        private void _deleteExpenseCompleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (SystemTray.ProgressIndicator != null)
                        SystemTray.ProgressIndicator.IsVisible = false;

                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (SystemTray.ProgressIndicator != null)
                        SystemTray.ProgressIndicator.IsVisible = false;

                    if (errorCode == HttpStatusCode.Unauthorized)
                    {
                        Util.logout();
                        NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show("Unable to delete expense", "Error", MessageBoxButton.OK);
                    }
                });
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

        }
    }
}