using Split_It_.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.ExpandableListHelper
{
    public class ExpandableListModel
    {
        public User groupUser { get; set; }
        public List<Debt_Group> debtList { get; set; }
        public bool isNonExpandable { get; set; }

        public ExpandableListModel()
        {
            debtList = new List<Debt_Group>();
            isNonExpandable = false;
        }
        
    }
}
