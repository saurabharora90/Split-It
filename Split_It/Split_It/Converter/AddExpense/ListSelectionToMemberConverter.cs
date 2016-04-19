using Split_It.Model;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.AddExpense
{
    public class ListSelectionToMemberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var e = value as SelectionChangedEventArgs;
            if (e.AddedItems.Count == 0)
                return null;
            return e.AddedItems[0] as ExpenseUser;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
