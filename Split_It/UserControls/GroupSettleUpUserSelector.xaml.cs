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
using System.Collections.ObjectModel;

namespace Split_It_.UserControls
{
    public partial class GroupSettleUpUserSelector : UserControl
    {
        Action<Debt_Group> Close;

        public GroupSettleUpUserSelector(List<Debt_Group> expenseUsers, Action<Debt_Group> close)
        {
            InitializeComponent();
            llsFriends.ItemsSource = expenseUsers;
            this.Close = close;
        }

        private void llsFriends_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (llsFriends.SelectedItem == null)
                return;

            Close(llsFriends.SelectedItem as Debt_Group);
        }
    }
}
