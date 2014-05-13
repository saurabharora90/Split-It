﻿using Split_It_.Model;
using Split_It_.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Split_It_.Converter
{
    public class BalanceToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush colorBrush;
            double finalBalance = 0;
            if (parameter != null && parameter.ToString().Equals("group"))
            {
                List<Debt_Group> allDebts = value as List<Debt_Group>;
                finalBalance = Util.getCurrentUserGroupDebtAmount(allDebts);
            }
            else
            {
                List<Balance_User> balance = value as List<Balance_User>;
                Balance_User defaultBalance = Util.getDefaultBalance(balance);
                finalBalance = System.Convert.ToDouble(defaultBalance.amount);
            }
            if (finalBalance > 0)
                colorBrush = Application.Current.Resources["positive"] as SolidColorBrush;
            else if(finalBalance == 0)
                colorBrush = Application.Current.Resources["settled"] as SolidColorBrush;
            else
                colorBrush = Application.Current.Resources["negative"] as SolidColorBrush;

            return colorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
