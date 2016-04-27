using Split_It.Model;
using Split_It.Utils;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter
{
    class ExpenseWhoPaidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) //This is for AddExpensePage
                return "You";

            var expenseUser = value as IEnumerable<ExpenseUser>;
            int numberOfPeoplePaid = 0;
            User paidUser = null;
            foreach (var user in expenseUser)
            {
                if (System.Convert.ToDouble(user.PaidShare) > 0)
                {
                    paidUser = user.User;
                    numberOfPeoplePaid++;
                }
            }
            if (paidUser == null) //Also for AddExpensePage
                return "You";

            else if (numberOfPeoplePaid > 1)
                return numberOfPeoplePaid.ToString() + " people";
            else
            {
                var user = AppState.CurrentUser;
                if (paidUser.id == user.id)
                    return "You";
                else
                    return paidUser.FirstName;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
