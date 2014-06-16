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
    public class AllDebtTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = "";
            ExpandableListModel expandableModel = value as ExpandableListModel;
            List<Debt_Group> allDebts = expandableModel.debtList;
            double finalBalance = Util.getUserGroupDebtAmount(allDebts, expandableModel.groupUser.id);

            //if final balance is 0, then anyways we are not shwoing the balance.
            if (finalBalance == 0)
                text = " is ";
            else if (finalBalance > 0)
                text = " is owed ";
            else if (finalBalance < 0)
                text = " owes ";

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
