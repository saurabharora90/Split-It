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
    public class GroupDebtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<Debt_Group> allDebts = value as List<Debt_Group>;
            double finalBalance = Util.getCurrentUserGroupDebtAmount(allDebts);
            string currency = allDebts[0].currency_code;
            string amount = currency + String.Format("{0:0.00}", Math.Abs(finalBalance));

            return amount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
