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
            List<Balance_User> balanceList = value as List<Balance_User>;
            bool hasMultipleBalances = Util.hasMultipleBalances(balanceList);
            
            Balance_User defaultBalance = Util.getDefaultBalance(balanceList);
            double finalBalance = System.Convert.ToDouble(defaultBalance.amount);
            if (finalBalance == 0)
                return null;
            else
            {
                string currency = defaultBalance.currency_code;
                string amount = currency + String.Format("{0:0.00}", Math.Abs(finalBalance));
                if (hasMultipleBalances)
                    return amount + "*";
                else
                    return amount;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
