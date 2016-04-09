using Microsoft.Practices.ServiceLocation;
using Split_It.Model;
using Split_It.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            List<GroupDebt> userDebts = new List<GroupDebt>();
            IEnumerable<GroupDebt> allDebts = null;
            if (group.SimplifyByDefault)
                allDebts = group.SimplifiedDebts;
            else
                allDebts = group.OriginalDebts;

            foreach (var debt in allDebts)
            {
                if (debt.From == user.id || debt.To == user.id)
                    userDebts.Add(debt);
            }
            if (userDebts.Count == 0)
                return String.Empty;

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

        public abstract Object getFinalValue(int position, IEnumerable<Friend> members, List<GroupDebt> userDebts);
    }
}
