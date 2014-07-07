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
    public class BalanceToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string description = "settled up";
            double finalBalance = 0;
            if (parameter != null && parameter.ToString().Equals("group"))
            {
                List<Debt_Group> allDebts = value as List<Debt_Group>;
                finalBalance = Util.getUserGroupDebtAmount(allDebts, App.currentUser.id);
            }
            else
            {
                List<Balance_User> balance = value as List<Balance_User>;
                Balance_User defaultBalance = Util.getDefaultBalance(balance);
                finalBalance = System.Convert.ToDouble(defaultBalance.amount, System.Globalization.CultureInfo.InvariantCulture);
            }
            if (finalBalance > 0)
                description = "owes you";
            else if (finalBalance < 0)
                description = "you owe";

            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
