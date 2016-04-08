using Microsoft.Practices.ServiceLocation;
using Split_It.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.UserBalance
{
    public abstract class BaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var balance = value as IEnumerable<Model.UserBalance>;
            var user = ServiceLocator.Current.GetInstance<MainViewModel>().CurrentUser;

            if (balance == null || balance.Count() == 0)
                return getFinalValue(null, 0);

            Model.UserBalance finalBalance = null;
            double amount;
            foreach (var currentBalance in balance)
            {
                if (finalBalance == null)
                    finalBalance = currentBalance;

                amount = System.Convert.ToDouble(currentBalance.Amount);
                if (currentBalance.CurrencyCode == user.DefaultCurrency && amount != 0)
                    finalBalance = currentBalance;
            }
            return getFinalValue(finalBalance, balance.Count());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public abstract Object getFinalValue(Model.UserBalance finalBalance, int numberOfBalances);
    }
}
