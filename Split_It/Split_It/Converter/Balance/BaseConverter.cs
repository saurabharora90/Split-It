using Microsoft.Practices.ServiceLocation;
using Split_It.Model;
using Split_It.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.Balance
{
    public abstract class BaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var user = ServiceLocator.Current.GetInstance<MainViewModel>().CurrentUser;
            IEnumerable<Model.UserBalance> balance = null;
            if (value is IEnumerable<Model.Friend>)
            {
                var members = value as IEnumerable<Friend>;
                Friend currentUserInGroup = null;
                foreach (var member in members)
                {
                    if (user.id == member.id)
                    {
                        currentUserInGroup = member;
                        break;
                    }
                }
                balance = currentUserInGroup.Balance;
            }
            else if (value is Model.UserBalance)
                return getFinalValue(value as Model.UserBalance, 1);
            else
                balance = value as IEnumerable<Model.UserBalance>;

            if (balance == null || balance.Count() == 0)
                return getFinalValue(null, 0);

            Model.UserBalance finalBalance = null;
            int balanceCount = 0;
            double amount;
            foreach (var currentBalance in balance)
            {
                if (finalBalance == null)
                    finalBalance = currentBalance;

                amount = System.Convert.ToDouble(currentBalance.Amount);
                if (currentBalance.CurrencyCode.Equals(user.DefaultCurrency, StringComparison.CurrentCultureIgnoreCase) && amount != 0)
                    finalBalance = currentBalance;

                if (amount != 0)
                    balanceCount++;
            }
            return getFinalValue(finalBalance, balanceCount);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public abstract Object getFinalValue(Model.UserBalance finalBalance, int numberOfBalances);
    }
}
