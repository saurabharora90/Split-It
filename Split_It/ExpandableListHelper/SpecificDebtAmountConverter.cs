using Split_It_.Model;
using Split_It_.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Split_It_.ExpandableListHelper
{
    public class SpecificDebtAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string amount = "";
            Debt_Group specificDebt = value as Debt_Group;
            double finalBalance = System.Convert.ToDouble(specificDebt.amount);

            string currency = specificDebt.currency_code;
            amount = currency + String.Format("{0:0.00}", Math.Abs(finalBalance));

            return amount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
