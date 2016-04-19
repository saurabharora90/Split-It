using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Split_It.Converter.AddExpense
{
    public class AvailableDetailsToBackgroudColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || String.IsNullOrEmpty(value.ToString()))
                return new SolidColorBrush(Colors.Black);
            else
                return Application.Current.Resources["splitwiseGreen"] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
