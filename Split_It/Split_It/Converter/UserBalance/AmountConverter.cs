using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Split_It.Model;

namespace Split_It.Converter.UserBalance
{
    public class AmountConverter : BaseConverter
    {
        public override object getFinalValue(Model.UserBalance finalBalance, int numberOfBalances)
        {
            double amount = System.Convert.ToDouble(finalBalance.Amount);
            //TODO: map currencycode to currency symbol
            string returnValue = finalBalance.CurrencyCode + Math.Abs(amount).ToString();
            if (numberOfBalances > 1)
                return returnValue += "*";
            else
                return returnValue;
        }
    }
}
