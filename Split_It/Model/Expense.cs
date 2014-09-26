﻿using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Model
{
    public class Expense
    {
        public static int DISPLAY_FOR_ALL_USER = 1000;
        public static int DISPLAY_FOR_SPECIFIC_USER = 1001;
        public static string DEFAULT_DETAILS = "No details associated with this expense.";

        /*public Expense()
        {
            displayType = DISPLAY_FOR_ALL_USER;
        }*/

        public int id { get; set; }
        public int group_id { get; set; }
        public string description { get; set; }

        private string _details;
        public string details
        {
            get
            {
                if (String.IsNullOrEmpty(_details))
                    return DEFAULT_DETAILS;
                else
                    return _details;
            }
            set { _details = value; }
        }

        //public string details { get; set; }
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

        //The following is used to help the ExpenseShareToAmountConverter
        //to determine if the amount is to be displayed as totaly for all users or as specific to one user
        //Eg: You booked a flight to KL to 7 ppl and paid for 490.
        //In all expenses, it will be shown as you lent 420 but in specific user it will be you lent 70
        [Ignore]
        public int specificUserId { get; set; }
        [Ignore]
        public int displayType { get; set; }
    }

    public class Expense_Share : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int expense_id { get; set; }
        
        [Ignore]
        public User user { get; set; }

        [Ignore]
        public string currency { get; set; }

        public int user_id { get; set; }

        private string _paidShare;
        public string paid_share
        {
            get { return _paidShare; }
            set
            {
                _paidShare = value;
                OnPropertyChanged("paid_share");
            }
        }

        private string _owedShare;
        public string owed_share
        {
            get { return _owedShare; }
            set
            {
                _owedShare = value;
                OnPropertyChanged("owed_share");
            }
        }

        public string net_balance { get; set; }


        //to help with spliting expense unequally
        [Ignore]
        public double percentage { get; set; }
        [Ignore]
        public double share { get; set; }

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            Expense_Share p = obj as Expense_Share;
            if ((System.Object)p == null)
            {
                return false;
            }

            return p.user_id == user_id;
        }

        public override int GetHashCode()
        {
            return user_id;
        }

        public override string ToString()
        {
            if (user_id == App.currentUser.id)
                return "You";
            else
                return user.first_name;
        }
    }
}
