using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Model
{
    public class ExpenseUser : ObservableObject
    {
        /// <summary>
        /// The <see cref="User" /> property's name.
        /// </summary>
        public const string UserPropertyName = "User";

        private User _user = null;

        /// <summary>
        /// Sets and gets the User property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public User User
        {
            get
            {
                return _user;
            }

            set
            {
                if (_user == value)
                {
                    return;
                }

                _user = value;
                RaisePropertyChanged(UserPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="UserId" /> property's name.
        /// </summary>
        public const string UserIdPropertyName = "UserId";

        private int _userID;

        /// <summary>
        /// Sets and gets the UserId property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int UserId
        {
            get
            {
                return _userID;
            }

            set
            {
                if (_userID == value)
                {
                    return;
                }

                _userID = value;
                RaisePropertyChanged(UserIdPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="PaidShare" /> property's name.
        /// </summary>
        public const string PaidSharePropertyName = "PaidShare";

        private string _paidShare = null;

        /// <summary>
        /// Sets and gets the PaidShare property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string PaidShare
        {
            get
            {
                return _paidShare;
            }

            set
            {
                if (_paidShare == value)
                {
                    return;
                }

                _paidShare = value;
                RaisePropertyChanged(PaidSharePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="OwedShare" /> property's name.
        /// </summary>
        public const string OwedSharePropertyName = "OwedShare";

        private string _owedShare = null;

        /// <summary>
        /// Sets and gets the OwedShare property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string OwedShare
        {
            get
            {
                return _owedShare;
            }

            set
            {
                if (_owedShare == value)
                {
                    return;
                }

                _owedShare = value;
                RaisePropertyChanged(OwedSharePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="NetBalance" /> property's name.
        /// </summary>
        public const string NetBalancePropertyName = "NetBalance";

        private string _netBalance = null;

        /// <summary>
        /// Sets and gets the NetBalance property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string NetBalance
        {
            get
            {
                return _netBalance;
            }

            set
            {
                if (_netBalance == value)
                {
                    return;
                }

                _netBalance = value;
                RaisePropertyChanged(NetBalancePropertyName);
            }
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = obj as ExpenseUser;
            if (other.UserId == UserId)
                return true;

            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }
    }
}
