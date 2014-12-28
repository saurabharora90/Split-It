using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Model
{
    public class Debt_Group
    {
        public int from { get; set; }
        public int to { get; set; }
        public string currency_code { get; set; }
        public string amount { get; set; }
        public int group_id { get; set; }

        [Ignore]
        public User fromUser { get; set; }
        [Ignore]
        public User toUser { get; set; }

        //the following is used in group summary expandable list
        [Ignore]
        public int ownerId { get; set; }
    }
}
