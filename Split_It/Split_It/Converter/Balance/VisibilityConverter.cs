using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Split_It.Model;
using Windows.UI.Xaml;

namespace Split_It.Converter.Balance
{
    public class VisibilityConverter : BaseConverter
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
