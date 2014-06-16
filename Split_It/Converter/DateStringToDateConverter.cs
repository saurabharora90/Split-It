using Split_It_.Model;
using Split_It_.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Split_It_.Converter
{
    public class DateStringToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string created_at = value as string;
            DateTime createdDate = DateTime.Parse(created_at, System.Globalization.CultureInfo.InvariantCulture);
            string dateString = createdDate.ToString("dd MMMM, yyyy", System.Globalization.CultureInfo.InvariantCulture);

            return dateString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
