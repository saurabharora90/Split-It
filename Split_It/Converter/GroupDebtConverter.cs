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
            string amount = "";
            List<Debt_Group> allDebts = value as List<Debt_Group>;
            double finalBalance = Util.getUserGroupDebtAmount(allDebts, App.currentUser.id);

            //if final balance is 0, then anyways we are not shwoing the balance.
            if (finalBalance != 0)
            {
                string currency = allDebts[0].currency_code;
                amount = currency + String.Format("{0:0.00}", Math.Abs(finalBalance));
            }

            return amount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
