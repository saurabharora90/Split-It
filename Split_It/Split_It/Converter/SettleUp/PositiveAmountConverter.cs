using System;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.SettleUp
{
    public class PositiveAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var amount = value.ToString();
            return Math.Abs(System.Convert.ToDouble(amount)).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
