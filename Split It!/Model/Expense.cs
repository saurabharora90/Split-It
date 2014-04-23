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
        public int friendship_id { get; set; }
        public int expense_bundle_id { get; set; }
        public string description { get; set; }
        public bool repeats { get; set; }
        public string repeat_interval { get; set; }
        public bool email_reminder { get; set; }
        public int email_reminder_in_advance { get; set; }
        public object next_repeat { get; set; }
        public string details { get; set; }
        public int comments_count { get; set; }
        public bool payment { get; set; }
        public string creation_method { get; set; }
        public string transaction_method { get; set; }
        public bool transaction_confirmed { get; set; }
        public string cost { get; set; }
        public string currency_code { get; set; }

        //Store as JSON in database
        public List<Debt> repayments { get; set; }
        public string date { get; set; }
        public string created_at { get; set; }

        //Store id of user in database
        public User created_by { get; set; }
        public string updated_at { get; set; }
        public object updated_by { get; set; }
        public object deleted_at { get; set; }
        public object deleted_by { get; set; }

        //Store id of category in database
        public Category category { get; set; }
        public Picture receipt { get; set; }

        //Store as JSON in database
        public List<ExpenseUser> users { get; set; }
    }

    public class ExpenseUser
    {
        public User user { get; set; }
        public int user_id { get; set; }
        public string paid_share { get; set; }
        public string owed_share { get; set; }
        public string net_balance { get; set; }
    }
}
