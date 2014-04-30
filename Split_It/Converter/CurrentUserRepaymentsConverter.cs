﻿using Split_It_.Model;
using Split_It_.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Split_It_.Converter
{
    public class CurrentUserRepaymentsConverter : IValueConverter
    {
        Expense expense;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            expense = value as Expense;
            List<Expense_Share> users = expense.users;

            Expense_Share currentUser = null;
            foreach (var user in users)
            {
                if (user.user_id == Util.getCurrentUserId())
                {
                    currentUser = user;
                    break;
                }
            }
            if (currentUser == null)
                return "You are not involved.";

            double netBalance = System.Convert.ToDouble(currentUser.net_balance);
            string amount = String.Format("{0:0.00}", Math.Abs(netBalance));

            if (netBalance > 0)
            {
                int peopleWhoOweYou = getNumberofPeopleWhoOweYou();
                if (peopleWhoOweYou > 1)
                {
                    return peopleWhoOweYou.ToString() + " people owe you " + expense.currency_code + amount + " for this.";
                }
                else
                {
                    User user = getUserWhoOwesYou();
                    return user.name + " owes you " + expense.currency_code + amount + " for this.";
                }
            }
            else
            {
                int peopleYouOweTo = getNumberofPeopleYouOweTo();
                if (peopleYouOweTo > 1)
                {
                    return "You owe " + peopleYouOweTo.ToString() + " people " + expense.currency_code + amount + " for this.";
                }
                else
                {
                    User user = getUserWhoYouOweTo();
                    return "You owe " + user.name + " " + expense.currency_code + amount + " for this.";
                }
            }
        }

        private User getUserWhoOwesYou()
        {
            int currentUserId = Util.getCurrentUserId();

            foreach (var repayment in expense.repayments)
            {
                if (repayment.to == currentUserId)
                {
                    return repayment.fromUser;
                }
            }
            return null;
        }

        private User getUserWhoYouOweTo()
        {
            int currentUserId = Util.getCurrentUserId();

            foreach (var repayment in expense.repayments)
            {
                if (repayment.from == currentUserId)
                {
                    return repayment.toUser;
                }
            }
            return null;
        }

        private int getNumberofPeopleWhoOweYou()
        {
            int peopleWhoOweYou = 0;
            int currentUserId = Util.getCurrentUserId();

            foreach (var repayment in expense.repayments)
            {
                if (repayment.to == currentUserId)
                {
                    peopleWhoOweYou++;
                }
            }
            return peopleWhoOweYou;
        }

        private int getNumberofPeopleYouOweTo()
        {
            int peopleYouOweTo = 0;
            int currentUserId = Util.getCurrentUserId();

            foreach (var repayment in expense.repayments)
            {
                if (repayment.from == currentUserId)
                {
                    peopleYouOweTo++;
                }
            }
            return peopleYouOweTo;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
