using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using Split_It_.Model;

namespace Split_It_.UserControls
{
    public partial class MultiplePayeeInputPopUpControl : UserControl
    {
        Action Close;
        public MultiplePayeeInputPopUpControl(ref ObservableCollection<Expense_Share> expenseUsers, Action close)
        {
            InitializeComponent();
            llsFriends.ItemsSource = expenseUsers;
            this.Close = close;
        }

        private void Okay_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Close();
        }
    }
}
