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
    public partial class UnequallySplit : UserControl
    {
        protected enum SplitUnequally { EXACT_AMOUNTS, PERCENTAGES, SHARES };
        protected SplitUnequally splitUnequally = SplitUnequally.EXACT_AMOUNTS;

        ObservableCollection<Expense_Share> expenseShareUsers;
        decimal expenseAmount;
        Action<ObservableCollection<Expense_Share>> Close;

        string decimalsep = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;

        public UnequallySplit(ObservableCollection<Expense_Share> users, decimal cost, Action<ObservableCollection<Expense_Share>> close)
        {
            InitializeComponent();
            this.expenseShareUsers = users;
            this.expenseAmount = cost;
            this.Close = close;

            this.llsExactAmount.ItemsSource = expenseShareUsers;
            this.llsPercentage.ItemsSource = expenseShareUsers;
            this.llsShares.ItemsSource = expenseShareUsers;
        }

        protected void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton btn = sender as RadioButton;

            if (llsExactAmount == null || llsPercentage == null || llsShares == null)
                return;

            if (btn.Content.ToString() == "amount")
            {
                splitUnequally = SplitUnequally.EXACT_AMOUNTS;
                llsExactAmount.Visibility = System.Windows.Visibility.Visible;
                llsPercentage.Visibility = System.Windows.Visibility.Collapsed;
                llsShares.Visibility = System.Windows.Visibility.Collapsed;
            }

            else if (btn.Content.ToString() == "percentage")
            {
                splitUnequally = SplitUnequally.PERCENTAGES;
                llsExactAmount.Visibility = System.Windows.Visibility.Collapsed;
                llsPercentage.Visibility = System.Windows.Visibility.Visible;
                llsShares.Visibility = System.Windows.Visibility.Collapsed;
            }

            else if (btn.Content.ToString() == "share")
            {
                splitUnequally = SplitUnequally.SHARES;
                llsExactAmount.Visibility = System.Windows.Visibility.Collapsed;
                llsPercentage.Visibility = System.Windows.Visibility.Collapsed;
                llsShares.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private bool divideExpenseUnequally()
        {
            bool proceed = true;
            int numberOfExpenseMembers = expenseShareUsers.Count;
            decimal extraAmount = 0; //this is extra amount left because of rounding off;
            int currentUserIndex = 0; //need the current user index to factor in the extra amount mentioned above
            decimal currenUserShareAmount = 0;

            switch (splitUnequally)
            {
                case SplitUnequally.EXACT_AMOUNTS:
                    decimal totalAmount = 0;

                    for (int i = 0; i < numberOfExpenseMembers; i++)
                    {
                        if (!String.IsNullOrEmpty(expenseShareUsers[i].owed_share))
                        {
                            if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(","))
                                expenseShareUsers[i].owed_share = expenseShareUsers[i].owed_share.Replace(".", ",");
                            else
                                expenseShareUsers[i].owed_share = expenseShareUsers[i].owed_share.Replace(",", ".");
                            decimal amount = Convert.ToDecimal(expenseShareUsers[i].owed_share);
                            totalAmount += amount;
                        }
                    }

                    if (totalAmount != expenseAmount)
                    {
                        tbError.Text = "The total amount split is not equal to expense amount.";
                        tbSum.Text = "Total: " + totalAmount + "/" + expenseAmount;
                        proceed = false;
                    }
                    break;
                case SplitUnequally.PERCENTAGES:
                    decimal totalPercentage = 0;

                    for (int i = 0; i < numberOfExpenseMembers; i++)
                    {
                        if (expenseShareUsers[i].user_id == App.currentUser.id)
                        {
                            currentUserIndex = i;
                        }
                        if (Utils.Util.isEmpty(expenseShareUsers[i].percentage))
                            expenseShareUsers[i].percentage = "0";

                        if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(","))
                            expenseShareUsers[i].percentage = expenseShareUsers[i].percentage.Replace(".", ",");
                        else
                            expenseShareUsers[i].percentage = expenseShareUsers[i].percentage.Replace(",", ".");

                        decimal shareAmount = expenseAmount * Convert.ToDecimal(expenseShareUsers[i].percentage) / 100;

                        //round off amount to digits
                        decimal shareAmountRounded = Math.Round(shareAmount, 2, MidpointRounding.AwayFromZero);
                        extraAmount += shareAmount - shareAmountRounded;

                        expenseShareUsers[i].owed_share = shareAmountRounded.ToString();

                        totalPercentage += Convert.ToDecimal(expenseShareUsers[i].percentage);
                    }

                    currenUserShareAmount = Convert.ToDecimal(expenseShareUsers[currentUserIndex].owed_share);
                    currenUserShareAmount += extraAmount;
                    expenseShareUsers[currentUserIndex].owed_share = currenUserShareAmount.ToString();

                    if (totalPercentage != 100)
                    {
                        tbError.Text = "Total percentage is not equal to 100%";
                        tbSum.Text = "Input Total: " + totalPercentage + "%";
                        proceed = false;
                    }
                    break;
                case SplitUnequally.SHARES:
                    decimal totalShares = 0;

                    for (int i = 0; i < numberOfExpenseMembers; i++)
                    {
                        if (Utils.Util.isEmpty(expenseShareUsers[i].share))
                            expenseShareUsers[i].share = "0";

                        if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(","))
                            expenseShareUsers[i].share = expenseShareUsers[i].share.Replace(".", ",");
                        else
                            expenseShareUsers[i].share = expenseShareUsers[i].share.Replace(",", ".");

                        totalShares += Convert.ToDecimal(expenseShareUsers[i].share);
                    }

                    if (totalShares == 0)
                    {
                        tbError.Text = "Total share cannot be 0";
                        tbSum.Text = "";
                        proceed = false;
                        return proceed;
                    }

                    decimal perShareAmount = expenseAmount / totalShares;
                    for (int i = 0; i < numberOfExpenseMembers; i++)
                    {
                        if (expenseShareUsers[i].user_id == App.currentUser.id)
                        {
                            currentUserIndex = i;
                        }
                        decimal shareAmount = Convert.ToDecimal(expenseShareUsers[i].share) * perShareAmount;

                        //round off amount to digits
                        decimal shareAmountRounded = Math.Round(shareAmount, 2, MidpointRounding.AwayFromZero);
                        extraAmount += shareAmount - shareAmountRounded;

                        expenseShareUsers[i].owed_share = shareAmountRounded.ToString();
                        expenseShareUsers[i].owed_share = shareAmountRounded.ToString();
                    }

                    currenUserShareAmount = Convert.ToDecimal(expenseShareUsers[currentUserIndex].owed_share);
                    currenUserShareAmount += extraAmount;
                    expenseShareUsers[currentUserIndex].owed_share = currenUserShareAmount.ToString();
                    break;
                default:
                    break;
            }

            return proceed;
        }

        private void Okay_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (divideExpenseUnequally())
                Close(expenseShareUsers);
        }

        protected void tbAmount_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            //do not allow user to input more than one decimal point
            if (textBox.Text.Contains(decimalsep) && (e.PlatformKeyCode == 190 || e.PlatformKeyCode == 188))
                e.Handled = true;
        }
    }
}
