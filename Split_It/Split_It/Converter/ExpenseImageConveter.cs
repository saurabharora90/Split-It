using System;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter
{
    public class ExpenseImageConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value == true)
                return "Assets/Images/expense_payment.png";
            else
                return "Assets/Images/expense_general.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
