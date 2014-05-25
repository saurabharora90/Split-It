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

namespace Split_It_.ExpandableListHelper
{
    public class SpecificDebtColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush colorBrush;
            Debt_Group specificDebt = value as Debt_Group;
            double finalBalance = System.Convert.ToDouble(specificDebt.amount);

            if (finalBalance > 0)
                colorBrush = Application.Current.Resources["positive"] as SolidColorBrush;
            else if (finalBalance == 0)
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