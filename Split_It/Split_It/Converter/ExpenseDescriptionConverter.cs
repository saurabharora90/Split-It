using Split_It.Model;
using System;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter
{
    public class ExpenseDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var expense = value as Expense;
            if (expense.Payment)
            {
                ExpenseUser paidUser = null, receivedUser = null;
                foreach (var user in expense.Users)
                {
                    if (System.Convert.ToDouble(user.PaidShare) != 0)
                        paidUser = user;
                    else if (System.Convert.ToDouble(user.OwedShare) != 0)
                        receivedUser = user;
                }
                return paidUser.User.FirstName + " paid " + receivedUser.User.FirstName + " " + expense.CurrencyCode + expense.Cost;
            }
            else
                return expense.Description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
