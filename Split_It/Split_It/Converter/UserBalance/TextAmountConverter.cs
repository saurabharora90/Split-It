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
    public class TextAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var balance = value as IEnumerable<Model.UserBalance>;
            var user = ServiceLocator.Current.GetInstance<MainViewModel>().CurrentUser;

            if (balance == null || balance.Count() == 0)
                return "settled up";

            Model.UserBalance finalBalance = null;
            double amount;
            foreach (var currentBalance in balance)
            {
                if(finalBalance == null)
                    finalBalance = currentBalance;

                amount = System.Convert.ToDouble(currentBalance.Amount);
                if (currentBalance.CurrencyCode == user.DefaultCurrency && amount != 0)
                    finalBalance = currentBalance;
            }

            //TODO: map currencycode to currency symbol
            amount = System.Convert.ToDouble(finalBalance.Amount);
            string returnValue = String.Empty;
            if (amount == 0)
                return "settled up";
            if (amount > 0)
                returnValue = "owes you";
            else
                returnValue = "you owe";

            returnValue += "\n" + finalBalance.CurrencyCode + Math.Abs(amount).ToString();

            if (balance.Count() > 1)
                return returnValue += "*";
            else
                return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
