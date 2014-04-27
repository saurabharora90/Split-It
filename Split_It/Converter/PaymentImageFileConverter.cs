using Split_It_.Model;
using Split_It_.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Split_It_.Converter
{
    /// <summary>
/// Caches the image that gets downloaded as part of Image control Source property.
/// </summary>
    public class PaymentImageFileConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Uri imageFileUri;
            bool payment = (bool)value;
            if (!payment)
            {
                imageFileUri = new Uri(@"Assets/Images/expense_general.png", UriKind.Relative);
            }
            else
            {
                imageFileUri = new Uri(@"Assets/Images/expense_payment.png", UriKind.Relative);
            }
            
            BitmapImage bm = new BitmapImage(imageFileUri);
            return bm;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
