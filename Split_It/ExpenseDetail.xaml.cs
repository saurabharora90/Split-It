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
using System.Collections.ObjectModel;

namespace Split_It_
{
    public partial class ExpenseDetail : PhoneApplicationPage
    {
        Expense selectedExpense;
        BackgroundWorker deleteExpenseBackgroundWorker;
        BackgroundWorker commentLoadingBackgroundWorker;
        BackgroundWorker addCommentBackgroundWorker;
        ApplicationBarIconButton btnAddComment;

        ObservableCollection<Comment> comments;

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

            commentLoadingBackgroundWorker = new BackgroundWorker();
            commentLoadingBackgroundWorker.WorkerSupportsCancellation = true;
            commentLoadingBackgroundWorker.DoWork += new DoWorkEventHandler(commentLoadingBackgroundWorker_DoWork);

            if (!commentLoadingBackgroundWorker.IsBusy)
            {
                commentLoadingBackgroundWorker.RunWorkerAsync();
            }

            addCommentBackgroundWorker = new BackgroundWorker();
            addCommentBackgroundWorker.WorkerSupportsCancellation = true;
            addCommentBackgroundWorker.DoWork += new DoWorkEventHandler(addCommentBackgroundWorker_DoWork);
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

            //add comment button
            btnAddComment = new ApplicationBarIconButton();
            btnAddComment.IconUri = new Uri("/Assets/Icons/add.png", UriKind.Relative);
            btnAddComment.Text = "add";
            ApplicationBar.Buttons.Add(btnAddComment);
            btnAddComment.Click += new EventHandler(btnAddComment_Click);
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
                            busyIndicator.IsRunning = true;

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
                    busyIndicator.IsRunning = true;
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

        private void commentLoadingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            CommentDatabase commentsObj = new CommentDatabase(_CommentsReceived);
            commentsObj.getComments(selectedExpense.id);
        }

        private void _CommentsReceived(List<Comment> commentList)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (commentList != null && commentList.Count != 0)
                {
                    comments = new ObservableCollection<Comment>();
                    foreach (var comment in commentList)
                    {
                        comments.Add(comment);
                    }
                }

                commentBusyIndicator.IsRunning = false;
            });
        }
        
        private void btnAddComment_Click(object sender, EventArgs e)
        {
            
        }

        private void addCommentBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }
        
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Pivot)sender).SelectedIndex)
            {
                case 2:
                    ApplicationBar.Buttons.Add(btnAddComment);
                    break;

                default:
                    ApplicationBar.Buttons.Remove(btnAddComment);
                    break;
            }
        }
    }
}