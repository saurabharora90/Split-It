using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Split_It.Converter.GroupBalance
{
    public class TextColorConverter : BaseMemberConverter
    {
        public override object getFinalValue(Model.UserBalance finalBalance, int numberOfBalances)
        {
            double amount = System.Convert.ToDouble(finalBalance.Amount);
            if (amount == 0)
                return Application.Current.Resources["settled"] as SolidColorBrush;
            else if (amount > 0)
                return Application.Current.Resources["positive"] as SolidColorBrush;
            else
                return Application.Current.Resources["negative"] as SolidColorBrush;
        }
    }
}
