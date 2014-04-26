using Split_It_.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Split_It_.Converter
{
    public class BalanceToAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<Balance_User> balance = value as List<Balance_User>;
            double finalBalance = getBalance(balance);
            if (finalBalance == 0)
                return null;
            else
            {
                string currency = balance[0].currency_code;
                return currency + " " + Math.Abs(finalBalance);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private double getBalance(List<Balance_User> balance)
        {
            double finalBalance = 0;

            if (balance == null)
                return finalBalance;

            foreach (var userBalance in balance)
            {
                
                finalBalance += System.Convert.ToDouble(userBalance.amount);
            }

            return finalBalance;
        }
    }
}
