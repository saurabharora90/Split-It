using System;
using Windows.UI.Text;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.Comment
{
    class CommentTypeFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value.ToString().Equals("system", StringComparison.CurrentCultureIgnoreCase))
                return FontWeights.Light;
            else
                return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
