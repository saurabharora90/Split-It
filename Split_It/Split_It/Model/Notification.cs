using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Model
{
    public class Notification : ObservableObject
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
        /// The <see cref="ImageUrl" /> property's name.
        /// </summary>
        public const string ImageUrlPropertyName = "ImageUrl";

        private string _imageUrl = null;

        /// <summary>
        /// Sets and gets the ImageUrl property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ImageUrl
        {
            get
            {
                return _imageUrl;
            }

            set
            {
                if (_imageUrl == value)
                {
                    return;
                }

                _imageUrl = value;
                RaisePropertyChanged(ImageUrlPropertyName);
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

        public DateTime? CreatedDate
        {
            get
            {
                return Convert.ToDateTime(CreatedAt);
            }
        }
    }
}
