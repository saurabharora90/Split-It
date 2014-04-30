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

namespace Split_It_
{
    public partial class ExpenseDetail : PhoneApplicationPage
    {
        Expense selectedExpense;

        public ExpenseDetail()
        {
            InitializeComponent();

            selectedExpense = PhoneApplicationService.Current.State[Constants.SELECTED_EXPENSE] as Expense;

            this.DataContext = selectedExpense;
        }
    }
}