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
        Action Close;

        public SelectPayeePopUpControl(ref ObservableCollection<Expense_Share> expenseShareUsers, Action close)
        {
            InitializeComponent();
            llsFriends.ItemsSource = expenseShareUsers;
            this.Close = close;
        }

        private void llsFriends_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Close();
        }

        private void tbMultiplePayers_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Close();
        }
    }
}
