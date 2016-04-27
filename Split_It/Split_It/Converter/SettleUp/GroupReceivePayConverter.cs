using Split_It.Model;
using Split_It.Utils;
using System;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.SettleUp
{
    public class GroupReceivePayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var debt = value as Debt;

            if (debt.From == AppState.CurrentUser.id)
                return "Pay";
            else
                return "Receive";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
     
    }
}
