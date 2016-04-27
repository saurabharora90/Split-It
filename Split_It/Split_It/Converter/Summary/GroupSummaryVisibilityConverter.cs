using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Split_It.Model;
using Windows.UI.Xaml;

namespace Split_It.Converter.Summary
{
    class GroupSummaryVisibilityConverter : BaseGroupSummaryConverter
    {
        public override object getFinalValue(int position, IEnumerable<Friend> members, List<Debt> userDebts)
        {
            if (position >= userDebts.Count)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }
    }
}
