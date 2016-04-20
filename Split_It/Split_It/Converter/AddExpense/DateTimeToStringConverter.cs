using System;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.AddExpense
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                DateTime date = System.Convert.ToDateTime(value);
                return new DateTimeOffset(date);
            }
            else
                return DateTimeOffset.Now;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            DateTimeOffset dto = (DateTimeOffset)value;
            return dto.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        }
    }
}
