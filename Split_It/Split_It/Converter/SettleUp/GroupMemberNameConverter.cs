using Split_It.Model;
using Split_It.Utils;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.SettleUp
{
    public class GroupMemberNameConverter : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var debt = value as Debt;
            string returnValue = null;
            int idToFind;

            if (debt.From == AppState.CurrenUserID)
            {
                idToFind = debt.To;
                returnValue = "to ";
            }
            else
            {
                idToFind = debt.From;
                returnValue = "from ";
            }

            foreach (var member in GroupMembers)
            {
                if (member.id == idToFind)
                    return returnValue + member.Name;
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Friend> GroupMembers
        {
            get { return (IEnumerable<Friend>)GetValue(GroupMembersProperty); }
            set { SetValue(GroupMembersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupMembers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupMembersProperty =
            DependencyProperty.Register("GroupMembers", typeof(IEnumerable<Friend>), typeof(GroupMemberNameConverter), new PropertyMetadata(0));


    }
}
