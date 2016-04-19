﻿using Split_It.Model;
using Split_It.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Split_It
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddExpensePage : Page
    {
        public AddExpensePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var dataContext = ((AddExpenseViewModel)(DataContext));
            if (e.Parameter is Expense)
                dataContext.ExpenseToAdd = e.Parameter as Expense;
        }
    }
}
