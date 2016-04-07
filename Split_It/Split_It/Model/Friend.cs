using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Model
{
    public class Friend : User
    {
        /// <summary>
        /// The <see cref="Balance" /> property's name.
        /// </summary>
        public const string BalancePropertyName = "Balance";

        private IEnumerable<UserBalance> _balance = null;

        /// <summary>
        /// Sets and gets the Balance property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IEnumerable<UserBalance> Balance
        {
            get
            {
                return _balance;
            }

            set
            {
                if (_balance == value)
                {
                    return;
                }

                _balance = value;
                RaisePropertyChanged(BalancePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Groups" /> property's name.
        /// </summary>
        public const string GroupsPropertyName = "Groups";

        private IEnumerable<Group> _groups = null;

        /// <summary>
        /// Sets and gets the Groups property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IEnumerable<Group> Groups
        {
            get
            {
                return _groups;
            }

            set
            {
                if (_groups == value)
                {
                    return;
                }

                _groups = value;
                RaisePropertyChanged(GroupsPropertyName);
            }
        }
    }
}
