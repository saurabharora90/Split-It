using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Split_It.Model;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;

namespace Split_It.Converter.Summary
{
    class GroupSummaryTextColorConverter : BaseGroupSummaryConverter
    {
        public override object getFinalValue(int position, IEnumerable<Friend> members, List<Debt> userDebts)
        {
            string returnText = String.Empty;

            if (position >= userDebts.Count)
                return Application.Current.Resources["settled"] as SolidColorBrush;
            else if (position <= 1)
            {
                var debt = userDebts[position];
                foreach (var member in members)
                {
                    if (member.id == debt.From) //this means current user is in "to" field
                    {
                        return Application.Current.Resources["positive"] as SolidColorBrush;
                    }
                    else if (member.id == debt.To)
                    {
                        return Application.Current.Resources["negative"] as SolidColorBrush;
                    }
                }
            }
            return Application.Current.Resources["settled"] as SolidColorBrush;
        }
    }
}
