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
    public class ExpenseToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                Expense expense = value as Expense;
                if (!expense.payment)
                    return expense.description;
                else
                {
                    List<Expense_Share> users = expense.users;
                    string paid = " paid";
                    string amount = getPaidByUser(users, System.Globalization.CultureInfo.InvariantCulture).paid_share;
                    return getPaidByUser(users, System.Globalization.CultureInfo.InvariantCulture).user.first_name + paid + " " + getPaidToUser(users, System.Globalization.CultureInfo.InvariantCulture).user.first_name + " " + expense.currency_code + System.Convert.ToDouble(amount, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch (Exception e)
            {
                return "Unable to get description";
            }
        }

        private Expense_Share getPaidByUser(List<Expense_Share> users, System.Globalization.CultureInfo culture)
        {
            foreach (var expenseUser in users)
            {
                if (System.Convert.ToDouble(expenseUser.paid_share, culture) > 0)
                {
                    return expenseUser;
                }
            }

            return null;
        }

        private Expense_Share getPaidToUser(List<Expense_Share> users, System.Globalization.CultureInfo culture)
        {
            foreach (var expenseUser in users)
            {
                if (System.Convert.ToDouble(expenseUser.paid_share, culture) == 0)
                {
                    return expenseUser;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
