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
        public string description
        {
            get {
                string description = "Settled Up";
                double finalBalance = getBalance();
                if (finalBalance > 0)
                    description = "owes you";
                else if (finalBalance < 0)
                    description = "you owe";

                return description;
            }
        }

        [Ignore]
        public Uri imageUrl
        {
            get
            {
                string url = @"Assets/Images/profilePhoto.png";
                if(picture == null)
                    return new Uri(url, UriKind.Relative);

                if ( picture.medium != null)
                    url = picture.medium;

                return new Uri(url);
            }
        }

     
        private double getBalance()
        {
            double finalBalance = 0;
            
            if (balance == null)
                return finalBalance;

            foreach (var userBalance in balance)
            {
                finalBalance += Convert.ToDouble(userBalance.amount);
            }

            return finalBalance;
        }

        public override string ToString()
        {
            double finalBalance = getBalance();
            if (finalBalance == 0)
                return null;
            else
            {
                string currency = balance[0].currency_code;
                return currency + " " + Math.Abs(finalBalance);
            }
        }
    }
}
