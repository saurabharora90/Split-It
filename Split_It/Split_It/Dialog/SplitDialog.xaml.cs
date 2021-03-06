﻿using Split_It.Model;
using Split_It.ViewModel;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Split_It.Dialog
{
    public sealed partial class SplitDialog : ContentDialog
    {
        public SplitDialog(Expense expense)
        {
            this.InitializeComponent();
            var dataContext = ((SplitDialogViewModel)(DataContext));
            dataContext.CurrentExpense = expense;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Hide();
        }
    }
}
