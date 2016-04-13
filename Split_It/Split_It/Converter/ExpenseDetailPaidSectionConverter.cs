using Split_It.Model;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter
{
    class ExpenseDetailPaidSectionConverter : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var expenseUser = value as ExpenseUser;
            string returnValue = expenseUser.User.Name + " ";
            if (System.Convert.ToDouble(expenseUser.PaidShare) != 0)
                returnValue += "paid " + CurrencyCode + expenseUser.PaidShare + " and ";

            returnValue += "owes " + CurrencyCode + expenseUser.OwedShare;
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }



        public string CurrencyCode
        {
            get { return (string)GetValue(CurrencyCodeProperty); }
            set { SetValue(CurrencyCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrencyCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrencyCodeProperty =
            DependencyProperty.Register("CurrencyCode", typeof(string), typeof(ExpenseDetailPaidSectionConverter), new PropertyMetadata(0));

    }
}
