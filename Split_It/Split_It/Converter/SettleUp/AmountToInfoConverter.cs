using System;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.SettleUp
{
    public class AmountToInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var amount = value.ToString();
            if (System.Convert.ToDouble(amount) > 0)
                return "Receive";
            else
                return "Pay";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
