using Split_It_.Model;
using Split_It_.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Split_It_.Converter
{
    public class ExpenseShareToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush colorBrush;

            List<Expense_Share> users = value as List<Expense_Share>;

            Expense_Share currentUser = null;
            foreach (var user in users)
            {
                if (user.user_id == Util.getCurrentUserId())
                {
                    currentUser = user;
                    break;
                }
            }

            if (currentUser == null)
                colorBrush = Application.Current.Resources["settled"] as SolidColorBrush;

            else if (System.Convert.ToDouble(currentUser.net_balance) > 0)
                colorBrush = Application.Current.Resources["positive"] as SolidColorBrush;

            else if (System.Convert.ToDouble(currentUser.net_balance) == 0)
                colorBrush = Application.Current.Resources["settled"] as SolidColorBrush;

            else
                colorBrush = Application.Current.Resources["negative"] as SolidColorBrush;

            return colorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
