using Microsoft.Practices.ServiceLocation;
using Split_It.Model;
using Split_It.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.GroupBalance
{
    public abstract class BaseMemberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var members = value as IEnumerable<Friend>;
            var user = ServiceLocator.Current.GetInstance<MainViewModel>().CurrentUser;
            Friend currentUserInGroup = null;
            foreach (var member in members)
            {
                if (user.id == member.id)
                {
                    currentUserInGroup = member;
                    break;
                }
            }

            Model.UserBalance finalBalance = null;
            double amount;
            int balanceCount = 0;
            foreach (var currentBalance in currentUserInGroup.Balance)
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
