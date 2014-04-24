using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Model
{
    public class Picture
    {
        public int user_id { get; set; }
        public string small { get; set; }
        public string medium { get; set; }
        public string large { get; set; }
        public object original { get; set; }
    }
}
