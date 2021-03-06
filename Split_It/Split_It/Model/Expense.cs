﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;

namespace Split_It.Model
{
    public class Expense : ObservableObject
    {
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
        /// The <see cref="Description" /> property's name.
        /// </summary>
        public const string DescriptionPropertyName = "Description";

        private string _description = null;

        /// <summary>
        /// Sets and gets the Description property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (_description == value)
                {
                    return;
                }

                _description = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Details" /> property's name.
        /// </summary>
        public const string DetailsPropertyName = "Details";

        private string _details = null;

        /// <summary>
        /// Sets and gets the Details property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Details
        {
            get
            {
                return _details;
            }

            set
            {
                if (_details == value)
                {
                    return;
                }

                _details = value;
                RaisePropertyChanged(DetailsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CommentsCount" /> property's name.
        /// </summary>
        public const string CommentCountPropertyName = "CommentsCount";

        private int _commentsCount;

        /// <summary>
        /// Sets and gets the CommentsCount property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int CommentsCount
        {
            get
            {
                return _commentsCount;
            }

            set
            {
                if (_commentsCount == value)
                {
                    return;
                }

                _commentsCount = value;
                RaisePropertyChanged(CommentCountPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Payment" /> property's name.
        /// </summary>
        public const string PaymentPropertyName = "Payment";

        private bool _payment = false;

        /// <summary>
        /// Sets and gets the Payment property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool Payment
        {
            get
            {
                return _payment;
            }

            set
            {
                if (_payment == value)
                {
                    return;
                }

                _payment = value;
                RaisePropertyChanged(PaymentPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Cost" /> property's name.
        /// </summary>
        public const string CostPropertyName = "Cost";

        private string _cost;

        /// <summary>
        /// Sets and gets the Cost property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Cost
        {
            get
            {
                return _cost;
            }

            set
            {
                if (_cost == value)
                {
                    return;
                }

                _cost = value;
                RaisePropertyChanged(CostPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CurrencyCode" /> property's name.
        /// </summary>
        public const string CurrencyCodePropertyName = "CurrencyCode";

        private string _currency_code;

        /// <summary>
        /// Sets and gets the CurrencyCode property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CurrencyCode
        {
            get
            {
                return _currency_code;
            }

            set
            {
                if (_currency_code == value)
                {
                    return;
                }

                _currency_code = value;
                RaisePropertyChanged(CurrencyCodePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Date" /> property's name.
        /// </summary>
        public const string DatePropertyName = "Date";

        private string _date;

        /// <summary>
        /// Sets and gets the Date property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Date
        {
            get
            {
                return _date;
            }

            set
            {
                if (_date == value)
                {
                    return;
                }

                _date = value;
                RaisePropertyChanged(DatePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CreatedAt" /> property's name.
        /// </summary>
        public const string CreatedAtPropertyName = "CreatedAt";

        private string _created_at;

        /// <summary>
        /// Sets and gets the CreatedAt property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CreatedAt
        {
            get
            {
                return _created_at;
            }

            set
            {
                if (_created_at == value)
                {
                    return;
                }

                _created_at = value;
                RaisePropertyChanged(CreatedAtPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CreatedBy" /> property's name.
        /// </summary>
        public const string CreatedByPropertyName = "CreatedBy";

        private User _createdBy = null;

        /// <summary>
        /// Sets and gets the CreatedBy property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public User CreatedBy
        {
            get
            {
                return _createdBy;
            }

            set
            {
                if (_createdBy == value)
                {
                    return;
                }

                _createdBy = value;
                RaisePropertyChanged(CreatedByPropertyName);
            }
        }

        /// <summary>
            /// The <see cref="UpdatedAt" /> property's name.
            /// </summary>
        public const string UpdatedAtPropertyName = "UpdatedAt";

        private string _upadatedAt;

        /// <summary>
        /// Sets and gets the UpdatedAt property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string UpdatedAt
        {
            get
            {
                return _upadatedAt;
            }

            set
            {
                if (_upadatedAt == value)
                {
                    return;
                }

                _upadatedAt = value;
                RaisePropertyChanged(UpdatedAtPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="UpdatedBy" /> property's name.
        /// </summary>
        public const string UpdatedByPropertyName = "UpdatedBy";

        private User _updatedBy = null;

        /// <summary>
        /// Sets and gets the UpdatedBy property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public User UpdatedBy
        {
            get
            {
                return _updatedBy;
            }

            set
            {
                if (_updatedBy == value)
                {
                    return;
                }

                _updatedBy = value;
                RaisePropertyChanged(UpdatedByPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Repayments" /> property's name.
        /// </summary>
        public const string RepaymentsPropertyName = "Repayments";

        private IEnumerable<Debt> _repayments = null;

        /// <summary>
        /// Sets and gets the Repayments property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IEnumerable<Debt> Repayments
        {
            get
            {
                return _repayments;
            }

            set
            {
                if (_repayments == value)
                {
                    return;
                }

                _repayments = value;
                RaisePropertyChanged(RepaymentsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Category" /> property's name.
        /// </summary>
        public const string CategoryPropertyName = "Category";

        private Category _category;

        /// <summary>
        /// Sets and gets the Category property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Category Category
        {
            get
            {
                return _category;
            }

            set
            {
                if (_category == value)
                {
                    return;
                }

                _category = value;
                RaisePropertyChanged(CategoryPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Receipt" /> property's name.
        /// </summary>
        public const string ReceiptPropertyName = "Receipt";

        private Photo _receipt = null;

        /// <summary>
        /// Sets and gets the Receipt property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Photo Receipt
        {
            get
            {
                return _receipt;
            }

            set
            {
                if (_receipt == value)
                {
                    return;
                }

                _receipt = value;
                RaisePropertyChanged(ReceiptPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Users" /> property's name.
        /// </summary>
        public const string UsersPropertyName = "Users";

        private IEnumerable<ExpenseUser> _users = null;

        /// <summary>
        /// Sets and gets the Users property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IEnumerable<ExpenseUser> Users
        {
            get
            {
                return _users;
            }

            set
            {
                if (_users == value)
                {
                    return;
                }

                _users = value;
                RaisePropertyChanged(UsersPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="DeletedAt" /> property's name.
        /// </summary>
        public const string DeletedAtPropertyName = "DeletedAt";

        private string _deletedAt = null;

        /// <summary>
        /// Sets and gets the DeletedAt property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DeletedAt
        {
            get
            {
                return _deletedAt;
            }

            set
            {
                if (_deletedAt == value)
                {
                    return;
                }

                _deletedAt = value;
                RaisePropertyChanged(DeletedAtPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CreationMethod" /> property's name.
        /// </summary>
        public const string CreationMethodPropertyName = "CreationMethod";

        private string _creationMethod = null;

        /// <summary>
        /// Sets and gets the CreationMethod property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CreationMethod
        {
            get
            {
                return _creationMethod;
            }

            set
            {
                if (_creationMethod == value)
                {
                    return;
                }

                _creationMethod = value;
                RaisePropertyChanged(CreationMethodPropertyName);
            }
        }

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

        #region HelperProperties
        public DateTime? CreatedDate
        {
            get
            {
                return Convert.ToDateTime(CreatedAt);
            }
        }

        public DateTime? UpdatedDate
        {
            get
            {
                if (String.IsNullOrEmpty(UpdatedAt))
                    return null;

                return Convert.ToDateTime(UpdatedAt);
            }
        }

        public DateTime? ExpenseDate
        {
            get
            {
                if (String.IsNullOrEmpty(Date))
                    return null;

                return Convert.ToDateTime(Date);
            }
        }

        #endregion

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = obj as Expense;
            if (Id == other.Id)
                return true;

            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
