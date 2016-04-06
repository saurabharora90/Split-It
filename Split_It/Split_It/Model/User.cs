using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Model
{
    public class User : GalaSoft.MvvmLight.ObservableObject
    {
        public int id { get; set; }

        /// <summary>
        /// The <see cref="FirstName" /> property's name.
        /// </summary>
        public const string FirstNamePropertyName = "FirstName";

        private string _firstName = null;

        /// <summary>
        /// Sets and gets the FirstName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        /// 
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName
        {
            get
            {
                return _firstName;
            }

            set
            {
                if (_firstName == value)
                {
                    return;
                }

                _firstName = value;
                RaisePropertyChanged(FirstNamePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="LastName" /> property's name.
        /// </summary>
        public const string LastNamePropertyName = "LastName";

        private string _lastName = null;

        /// <summary>
        /// Sets and gets the LastName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        /// 
        [JsonProperty(PropertyName = "last_name")]
        public string LastName
        {
            get
            {
                return _lastName;
            }

            set
            {
                if (_lastName == value)
                {
                    return;
                }

                _lastName = value;
                RaisePropertyChanged(LastNamePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Email" /> property's name.
        /// </summary>
        public const string EmailPropertyName = "Email";

        private string _email = null;

        /// <summary>
        /// Sets and gets the Email property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        /// 
        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                if (_email == value)
                {
                    return;
                }

                _email = value;
                RaisePropertyChanged(EmailPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Picture" /> property's name.
        /// </summary>
        public const string PicturePropertyName = "Picture";

        private Photo _picture = null;

        /// <summary>
        /// Sets and gets the Picture property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        /// 
        [JsonProperty(PropertyName = "picture")]
        public Photo Picture
        {
            get
            {
                return _picture;
            }

            set
            {
                if (_picture == value)
                {
                    return;
                }

                _picture = value;
                RaisePropertyChanged(PicturePropertyName);
            }
        }
    }
}
