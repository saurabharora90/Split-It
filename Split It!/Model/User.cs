using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Model
{
    public class User
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public Picture picture { get; set; }
        public string email { get; set; }
        public string registration_status { get; set; }
        public string country_code { get; set; }
        public string default_currency { get; set; }

        //Store as JSON in database
        public List<Debt> balance { get; set; }

        //Have a user_group table which stores the links between user and groups
        public List<Group> groups { get; set; }
        public string updated_at { get; set; }
    }
}
