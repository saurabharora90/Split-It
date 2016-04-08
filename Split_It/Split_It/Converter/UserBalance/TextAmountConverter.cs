using Microsoft.Practices.ServiceLocation;
using Split_It.Model;
using Split_It.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.UserBalance
{
    public class TextAmountConverter : BaseConverter
    {
        public override object getFinalValue(Model.UserBalance finalBalance, int numberOfBalances)
        {
            if (finalBalance == null)
                return "settle up";

            //TODO: map currencycode to currency symbol
            double amount = System.Convert.ToDouble(finalBalance.Amount);
            string returnValue = String.Empty;
            if (amount == 0)
                return "settled up";
            if (amount > 0)
                returnValue = "owes you";
            else
                returnValue = "you owe";

            returnValue += "\n" + finalBalance.CurrencyCode + Math.Abs(amount).ToString();

            if (numberOfBalances > 1)
                return returnValue += "*";
            else
                return returnValue;
        }
    }
}
