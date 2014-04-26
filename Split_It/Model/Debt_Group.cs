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
    }
}
