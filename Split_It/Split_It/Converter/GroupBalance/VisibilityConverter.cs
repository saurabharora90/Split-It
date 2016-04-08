using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Split_It.Converter.GroupBalance
{
    public class VisibilityConverter : BaseMemberConverter
    {
        public override object getFinalValue(Model.UserBalance finalBalance, int numberOfBalances)
        {
            double amount = System.Convert.ToDouble(finalBalance.Amount);
            if (amount == 0)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }
    }
}
