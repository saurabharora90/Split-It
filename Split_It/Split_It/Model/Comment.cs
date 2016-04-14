using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Model
{
    public class Comment : ObservableObject
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
        /// The <see cref="Content" /> property's name.
        /// </summary>
        public const string ContentPropertyName = "Content";

        private string _content = null;

        /// <summary>
        /// Sets and gets the Content property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Content
        {
            get
            {
                return _content;
            }

            set
            {
                if (_content == value)
                {
                    return;
                }

                _content = value;
                RaisePropertyChanged(ContentPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CommentType" /> property's name.
        /// </summary>
        public const string CommentTypePropertyName = "CommentType";

        private string _commentType = null;

        /// <summary>
        /// Sets and gets the CommentType property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CommentType
        {
            get
            {
                return _commentType;
            }

            set
            {
                if (_commentType == value)
                {
                    return;
                }

                _commentType = value;
                RaisePropertyChanged(CommentTypePropertyName);
            }
        }

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
    }
}
