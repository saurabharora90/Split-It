using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Model
{
    //[Ignore] for attributes that are not a part of the database
    public class User
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }

        [Ignore]
        public Picture picture { get; set; }
        
        public string email { get; set; }
        public string country_code { get; set; }
        public string default_currency { get; set; }

        [Ignore]
        public List<Balance_User> balance { get; set; }

        public string updated_at { get; set; }

        [Ignore]
        public string name
        {
            get
            {
                if (!String.IsNullOrEmpty(last_name))
                    return first_name + " " + last_name;
                else
                    return first_name;
            }
        }
    }
}
