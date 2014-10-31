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
    public class ExpenseShareToRepaymentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Expense_Share shareUser = value as Expense_Share;
            string currency = shareUser.currency;
            double paidShare = System.Convert.ToDouble(shareUser.paid_share, System.Globalization.CultureInfo.InvariantCulture);
            double owedShare = System.Convert.ToDouble(shareUser.owed_share, System.Globalization.CultureInfo.InvariantCulture);
            string username;

            if (shareUser.user == null)
                username = "Unknown user (not a friend)";
            else
                username = shareUser.user.first_name;
            string result;

            if (paidShare > 0)
            {
                result = username + " paid " + currency + String.Format("{0:0.00}", paidShare);

                if (owedShare > 0)
                {
                    result += " and owes " + currency + String.Format("{0:0.00}", owedShare);
                }

            }
            else
            {
                result = username + " owes " + currency + String.Format("{0:0.00}", owedShare);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
