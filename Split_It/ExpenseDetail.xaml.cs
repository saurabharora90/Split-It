﻿using System;
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
using Telerik.Windows.Controls;

namespace Split_It_
{
    public partial class ExpenseDetail : PhoneApplicationPage
    {
        Expense selectedExpense;
        BackgroundWorker deleteExpenseBackgroundWorker;
        BackgroundWorker commentLoadingBackgroundWorker;
        BackgroundWorker addCommentBackgroundWorker;

        ObservableCollection<CustomCommentView> comments = new ObservableCollection<CustomCommentView>();

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

            this.conversationView.ItemsSource = this.comments;
            this.conversationView.CreateMessage = (string text) => new CustomCommentView(text, DateTime.Now, App.currentUser.name);
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

            //can only edit expenses for the time being
            if (selectedExpense.payment)
                btnEdit.IsEnabled = false;
            else
                btnEdit.IsEnabled = true;
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
                        MessageBox.Show("Unable to delete expense", "Error", MessageBoxButton.OK);
                    }
                });
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //currently you cannot edit all expenses.
            if (canEditExpense())
            {
                PhoneApplicationService.Current.State[Constants.ADD_EXPENSE] = selectedExpense;
                NavigationService.Navigate(new Uri("/Add_Expense_Pages/EditExpense.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("This expense has an unknown user (not a friend). You cannnot edit such an expense in this version. This facility will be added in a future update"
                    , "Sorry", MessageBoxButton.OK);
            }
        }

        private bool canEditExpense()
        {
            foreach (var item in selectedExpense.users)
            {
                //user is null when the user is not a friend and hence does not exist in the DB.
                if (item.user == null)
                    return false;
            }
            return true;
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
                    foreach (var comment in commentList)
                    {
                        if (String.IsNullOrEmpty(comment.deleted_at))
                        {
                            DateTime createdDate = DateTime.Parse(comment.created_at, System.Globalization.CultureInfo.InvariantCulture);
                            comments.Add(new CustomCommentView(comment.content, createdDate, comment.user.name));
                        }
                    }
                }

                busyIndicator.IsRunning = false;
            });
        }

        private void addCommentBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string content = (e.Argument as CustomCommentView).Text;
            CommentDatabase commentsObj = new CommentDatabase(_CommentsReceived);
            commentsObj.addComment(selectedExpense.id, content);
        }

        private void OnSendingMessage(object sender, ConversationViewMessageEventArgs e)
        {
            if (string.IsNullOrEmpty((e.Message as CustomCommentView).Text))
            {
                return;
            }

            //send this message to the api via the add comment background worker
            if (!addCommentBackgroundWorker.IsBusy)
            {
                this.Focus();
                busyIndicator.IsRunning = true;
                addCommentBackgroundWorker.RunWorkerAsync(e.Message);
            }
        }

        public class CustomCommentView : ConversationViewMessage
        {
            public string name { get; set; }

            public CustomCommentView(string text, DateTime timeStamp, string userName)
                : base(text, timeStamp, ConversationViewMessageType.Incoming)
            {
                this.name = userName;
            }

            public string FormattedTimeStamp
            {
                get
                {
                    return this.TimeStamp.ToLongDateString();
                }
            }
        }
    }
}