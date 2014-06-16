using Split_It_.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Split_It_.Converter.UserDetails
{
    public class UserDetailsExpensesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string description = "settled up";
            double finalBalance = 0;
            Balance_User balance = value as Balance_User;
            finalBalance = System.Convert.ToDouble(balance.amount, culture);
            string amount = balance.currency_code + String.Format("{0:0.00}", Math.Abs(finalBalance));
            
            if (finalBalance > 0)
                description = "owes you " + amount;
            else if (finalBalance < 0)
                description = "you owe " + amount;

            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
