using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Model
{
    public class Group
    {
        public int id { get; set; }
        public string name { get; set; }
        public string updated_at { get; set; }
        public List<User> members { get; set; }
        public bool simplify_by_default { get; set; }
        public List<Debt> original_debts { get; set; }
        public List<Debt> simplified_debts { get; set; }
        public object whiteboard { get; set; }
        public string group_type { get; set; }
    }
}
