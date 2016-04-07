using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Model
{
    public class Group : ObservableObject
    {
        /// <summary>
        /// The <see cref="GroupId" /> property's name.
        /// </summary>
        public const string GroupIdPropertyName = "GroupId";

        private int _groupId;

        /// <summary>
        /// Sets and gets the GroupId property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int GroupId
        {
            get
            {
                return _groupId;
            }

            set
            {
                if (_groupId == value)
                {
                    return;
                }

                _groupId = value;
                RaisePropertyChanged(GroupIdPropertyName);
            }
        }

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
    }
}
