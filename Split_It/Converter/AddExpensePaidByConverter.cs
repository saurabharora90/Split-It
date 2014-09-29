using Split_It_.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Split_It_.Converter
{
    public class AddExpensePaidByConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<Expense_Share> expenseShareUsers = value as List<Expense_Share>;

            //This condition will be met when the page is first fired as that time the hasPaid for the current user has not been set.
            if (expenseShareUsers == null || expenseShareUsers.Count == 1)
                return "You";
            else
            {
                int count = 0;
                Expense_Share paidUser = null;
                foreach (var expenseUser in expenseShareUsers)
                {
                    if (expenseUser.hasPaid)
                    {
                        count++;
                        paidUser = expenseUser;
                    }
                }

                if (count == 1)
                    return paidUser.ToString();
                else
                    return "Multiple People (" + count.ToString() + ")";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
