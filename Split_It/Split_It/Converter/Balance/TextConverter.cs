using System;

namespace Split_It.Converter.Balance
{
    public class TextConverter : BaseConverter
    {
        public override object getFinalValue(Model.UserBalance finalBalance, int numberOfBalances)
        {
            if (finalBalance == null)
                return "settled up";

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
