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
    public class BalanceToAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<Balance_User> balance = value as List<Balance_User>;
            double finalBalance = Util.getBalance(balance);
            if (finalBalance == 0)
                return null;
            else
            {
                string currency = balance[0].currency_code;
                return currency + String.Format("{0:0.00}", Math.Abs(finalBalance));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
}
