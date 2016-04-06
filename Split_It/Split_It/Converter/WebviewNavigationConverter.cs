using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter
{
    public class WebviewNavigationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            Uri uri;
            if(value is WebViewNavigationCompletedEventArgs)
            {
                var args = (WebViewNavigationCompletedEventArgs)value;
                uri = args.Uri;
            }
                
            else
            {
                var args = (WebViewNavigationStartingEventArgs)value;
                uri = args.Uri;
            }
                
            return uri.AbsoluteUri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
