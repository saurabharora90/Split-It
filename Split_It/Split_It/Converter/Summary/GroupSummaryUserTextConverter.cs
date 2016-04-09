using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Split_It.Model;

namespace Split_It.Converter.Summary
{
    class GroupSummaryUserTextConverter : BaseGroupSummaryConverter
    {
        public override object getFinalValue(int position, IEnumerable<Friend> members, List<GroupDebt> userDebts)
        {
            string returnText = String.Empty;

            if (position >= userDebts.Count)
                returnText = String.Empty;    
            else if(position <=1)
            {
                var debt = userDebts[position];
                foreach (var member in members)
                {
                    if (member.id == debt.From) //this means current user is in "to" field
                    {
                        returnText = member.FirstName + " owes you";
                        break;
                    }
                    else if(member.id == debt.To)
                    {
                        returnText = "You owe " + member.FirstName;
                        break;
                    }
                }
            }
            else
            {
                int leftCount = userDebts.Count - position;
                returnText = "Plus " + leftCount + " other balances";
            }
            return returnText;
        }
    }
}
