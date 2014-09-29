using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using Split_It_.Model;
using Telerik.Windows.Controls;

namespace Split_It_.UserControls
{
    public partial class SelectPayeePopUpControl : UserControl
    {
        Action<Expense_Share, bool> Close;
        ObservableCollection<Expense_Share> expenseShareUsers;

        public SelectPayeePopUpControl(ref ObservableCollection<Expense_Share> expenseUsers, Action<Expense_Share, bool> close)
        {
            InitializeComponent();
            this.expenseShareUsers = expenseUsers;
            llsFriends.ItemsSource = this.expenseShareUsers;
            this.Close = close;
        }

        private void llsFriends_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (llsFriends.SelectedItem == null)
                return;

            Close(llsFriends.SelectedItem as Expense_Share, false);
        }

        private void tbMultiplePayers_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Close(null, true);
        }
    }
}
