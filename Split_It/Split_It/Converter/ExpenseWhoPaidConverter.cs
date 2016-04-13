﻿using Split_It.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter
{
    class ExpenseWhoPaidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
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
            if (numberOfPeoplePaid > 1)
                return numberOfPeoplePaid.ToString() + " people";
            else
                return paidUser.FirstName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}