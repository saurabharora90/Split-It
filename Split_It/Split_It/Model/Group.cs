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
        /// The <see cref="Id" /> property's name.
        /// </summary>
        public const string IdPropertyName = "Id";

        private int _id;

        /// <summary>
        /// Sets and gets the Id property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id == value)
                {
                    return;
                }

                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _name = null;

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="UpdatedAt" /> property's name.
        /// </summary>
        public const string UpdatedAtPropertyName = "UpdatedAt";

        private string _updatedAt = null;

        /// <summary>
        /// Sets and gets the UpdatedAt property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string UpdatedAt
        {
            get
            {
                return _updatedAt;
            }

            set
            {
                if (_updatedAt == value)
                {
                    return;
                }

                _updatedAt = value;
                RaisePropertyChanged(UpdatedAtPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Members" /> property's name.
        /// </summary>
        public const string MembersPropertyName = "Members";

        private IEnumerable<Friend> _members = null;

        /// <summary>
        /// Sets and gets the Members property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IEnumerable<Friend> Members
        {
            get
            {
                return _members;
            }

            set
            {
                if (_members == value)
                {
                    return;
                }

                _members = value;
                RaisePropertyChanged(MembersPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SimplifyByDefault" /> property's name.
        /// </summary>
        public const string SimplifyByDefaultPropertyName = "SimplifyByDefault";

        private bool _simplifyByDefault = false;

        /// <summary>
        /// Sets and gets the SimplifyByDefault property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool SimplifyByDefault
        {
            get
            {
                return _simplifyByDefault;
            }

            set
            {
                if (_simplifyByDefault == value)
                {
                    return;
                }

                _simplifyByDefault = value;
                RaisePropertyChanged(SimplifyByDefaultPropertyName);
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

        /// <summary>
        /// The <see cref="OriginalDebts" /> property's name.
        /// </summary>
        public const string OriginalDebtsPropertyName = "OriginalDebts";

        private IEnumerable<GroupDebt> _originalDebts = null;

        /// <summary>
        /// Sets and gets the OriginalDebts property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IEnumerable<GroupDebt> OriginalDebts
        {
            get
            {
                return _originalDebts;
            }

            set
            {
                if (_originalDebts == value)
                {
                    return;
                }

                _originalDebts = value;
                RaisePropertyChanged(OriginalDebtsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SimplifiedDebts" /> property's name.
        /// </summary>
        public const string SimplifiedDebtsPropertyName = "SimplifiedDebts";

        private IEnumerable<GroupDebt> _simplifiedDebts = null;

        /// <summary>
        /// Sets and gets the SimplifiedDebts property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IEnumerable<GroupDebt> SimplifiedDebts
        {
            get
            {
                return _simplifiedDebts;
            }

            set
            {
                if (_simplifiedDebts == value)
                {
                    return;
                }

                _simplifiedDebts = value;
                RaisePropertyChanged(SimplifiedDebtsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="GroupType" /> property's name.
        /// </summary>
        public const string GroupTypePropertyName = "GroupType";

        private string _groupType = null;

        /// <summary>
        /// Sets and gets the GroupType property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string GroupType
        {
            get
            {
                return _groupType;
            }

            set
            {
                if (_groupType == value)
                {
                    return;
                }

                _groupType = value;
                RaisePropertyChanged(GroupTypePropertyName);
            }
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Group other = obj as Group;
            if (Id == other.Id || Id == other.GroupId || GroupId == other.GroupId)
                return true;

            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            throw new NotImplementedException();
            return base.GetHashCode();
        }
    }
}
