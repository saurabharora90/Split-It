using Microsoft.Practices.ServiceLocation;
using Split_It.Model;
using Split_It.ViewModel;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.Summary
{
    public abstract class BaseGroupSummaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var group = value as Group;
            var user = ServiceLocator.Current.GetInstance<MainViewModel>().CurrentUser;
            //find current user in group.
            List<Debt> userDebts = new List<Debt>();
            IEnumerable<Debt> allDebts = null;
            if (group.SimplifyByDefault)
                allDebts = group.SimplifiedDebts;
            else
                allDebts = group.OriginalDebts;

            foreach (var debt in allDebts)
            {
                if (debt.From == user.id || debt.To == user.id)
                    userDebts.Add(debt);
            }

            int position = System.Convert.ToInt32(parameter);
            List<Friend> otherGroupMembers = new List<Friend>();
            foreach (var member in group.Members)
            {
                if (member.id != user.id)
                    otherGroupMembers.Add(member);
            }
            return getFinalValue(position, otherGroupMembers, userDebts);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="members"></param> List of all other group members (excluding the current user).
        /// <param name="userDebts"></param> The debts in which the current user is involved.
        /// <returns></returns>
        public abstract Object getFinalValue(int position, IEnumerable<Friend> members, List<Debt> userDebts);
    }
}
