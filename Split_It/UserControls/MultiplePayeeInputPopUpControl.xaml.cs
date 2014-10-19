﻿using System;
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
using System.Globalization;

namespace Split_It_.UserControls
{
    public partial class MultiplePayeeInputPopUpControl : UserControl
    {
        Action Close;
        double ExpenseCost;

        string decimalsep = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;

        public MultiplePayeeInputPopUpControl(ref ObservableCollection<Expense_Share> expenseUsers, Action close, double total)
        {
            InitializeComponent();
            llsFriends.ItemsSource = expenseUsers;
            this.ExpenseCost = total;
            this.Close = close;
        }

        private void Okay_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if(calculateTotalInput())
                Close();
        }

        private bool calculateTotalInput()
        {
            ObservableCollection<Expense_Share> expenseUsers = llsFriends.ItemsSource as ObservableCollection<Expense_Share>;
            double total = 0;
            foreach (var item in expenseUsers)
            {
                if (!String.IsNullOrEmpty(item.paid_share))
                {
                    total += Convert.ToDouble(item.paid_share);
                }
            }

            if (ExpenseCost == total)
                return true;

            else
            {
                tbError.Text = "The paid amount for each person do not add up to the total cost of the bill.";
                tbSum.Text = "Total: " + total + "/" + ExpenseCost;
                return false;
            }
        }

        protected void tbAmount_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            PhoneTextBox textBox = sender as PhoneTextBox;

            //do not allow user to input more than one decimal point
            if (textBox.Text.Contains(decimalsep) && (e.PlatformKeyCode == 190 || e.PlatformKeyCode == 188))
                e.Handled = true;
        }
    }
}