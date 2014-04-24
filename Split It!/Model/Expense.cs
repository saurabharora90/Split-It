using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Model
{
    class Expense
    {
        public int id { get; set; }
        public int group_id { get; set; }
        public string description { get; set; }
        public string details { get; set; }
        public bool payment { get; set; }
        public string creation_method { get; set; }
        public bool transaction_confirmed { get; set; }
        public string cost { get; set; }
        public string currency_code { get; set; }

        [Ignore]
        public List<Debt_Expense> repayments { get; set; }
        
        public string date { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

        [Column("created_by")]
        public int created_by_user_id { get; set; }

        [Column("updated_by")]
        public int updated_by_user_id { get; set; }

        [Column("deleted_by")]
        public int deleted_by_user_id { get; set; }

        [Ignore]
        public User created_by { get; set; }
        [Ignore]
        public User updated_by { get; set; }
        [Ignore]
        public User deleted_by { get; set; }
        [Ignore]
        public Picture receipt { get; set; }
        [Ignore]
        public Category category { get; set; }
        [Ignore]
        public List<Expense_Share> users { get; set; }
    }

    public class Expense_Share
    {
        public int expense_id { get; set; }
        
        [Ignore]
        public User user { get; set; }
        
        public int user_id { get; set; }
        public string paid_share { get; set; }
        public string owed_share { get; set; }
        public string net_balance { get; set; }
    }
}
