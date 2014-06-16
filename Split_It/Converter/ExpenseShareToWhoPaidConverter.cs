using Split_It_.Model;
using Split_It_.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Split_It_.Converter
{
    public class ExpenseShareToWhoPaidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Expense expense = value as Expense;
            List<Expense_Share> users = expense.users;

            Expense_Share paidUser = null;
            foreach (var user in users)
            {
                if (System.Convert.ToDouble(user.paid_share, culture) > 0)
                {
                    paidUser = user;
                    break;
                }
            }

            if (checkIfNotInvolved(users))
            {
                return "You are not involved";
            }

            if (paidUser == null)
                return null;

            string amount = null;
            amount = String.Format("{0:0.00}", Math.Abs(System.Convert.ToDouble(paidUser.paid_share, culture)));
            string paid = " paid";

            return getPaidUserName(paidUser.user) + paid + " " + expense.currency_code + amount;
        }

        private String getPaidUserName(User paidUser)
        {
            if (paidUser.id == Util.getCurrentUserId())
                return "You";
            else
                return paidUser.first_name;
        }

        private bool checkIfNotInvolved(List<Expense_Share> users)
        {
            foreach (var user in users)
            {
                if (user.user_id == Util.getCurrentUserId())
                {
                    return false;
                }
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
