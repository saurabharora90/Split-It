using Microsoft.Practices.ServiceLocation;
using Split_It.Model;
using Split_It.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.Balance
{
    public class TextConverter : BaseConverter
    {
        public override object getFinalValue(Model.UserBalance finalBalance, int numberOfBalances)
        {
            if (finalBalance == null)
                return "settle up";

            double amount = System.Convert.ToDouble(finalBalance.Amount);
            string returnValue = String.Empty;
            if (amount == 0)
                returnValue = "settled up";
            else if (amount > 0)
                returnValue = "owes you";
            else
                returnValue = "you owe";

            return returnValue;
        }
    }
}
