using Split_It.Model;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.AddExpense
{
    public class GroupSelectedToMembersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var e = value as SelectionChangedEventArgs;
            if (e.AddedItems.Count == 0)
                return null;

            var group = e.AddedItems[0] as Group;
            if (group.Id == 0)
                return null;
            else
                return group.Members;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
