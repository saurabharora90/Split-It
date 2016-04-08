using Microsoft.Practices.ServiceLocation;
using Split_It.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.UserBalance
{
    abstract class BaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var balance = value as IEnumerable<Model.UserBalance>;
            var user = ServiceLocator.Current.GetInstance<MainViewModel>().CurrentUser;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public abstract Object getFinalValue(Model.UserBalance finalBalance, int totalBalance);
    }
}
