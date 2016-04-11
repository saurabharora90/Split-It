using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Split_It.Model;

namespace Split_It.Converter.Summary
{
    class GroupSummaryAmountConverter : BaseGroupSummaryConverter
    {
        public override object getFinalValue(int position, IEnumerable<Friend> members, List<Debt> userDebts)
        {
            string returnText = String.Empty;

            if (position >= userDebts.Count)
                returnText = String.Empty;    
            else if(position <=1)
            {
                var debt = userDebts[position];
                returnText = debt.CurrencyCode + debt.Amount;
            }
            else
            {
                returnText = String.Empty;
            }
            return returnText;
        }
    }
}
