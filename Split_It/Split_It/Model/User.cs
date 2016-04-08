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
                RaisePropertyChanged(NamePropertyName);
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
                RaisePropertyChanged(NamePropertyName);
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

        /// <summary>
        /// The <see cref="CountryCode" /> property's name.
        /// </summary>
        public const string CountryCodePropertyName = "CountryCode";

        private string _countryCode = null;

        /// <summary>
        /// Sets and gets the CountryCode property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CountryCode
        {
            get
            {
                return _countryCode;
            }

            set
            {
                if (_countryCode == value)
                {
                    return;
                }

                _countryCode = value;
                RaisePropertyChanged(CountryCodePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="DefaultCurrency" /> property's name.
        /// </summary>
        public const string DefaultCurrencyPropertyName = "DefaultCurrency";

        private string _defaultCurrency = null;

        /// <summary>
        /// Sets and gets the DefaultCurrency property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DefaultCurrency
        {
            get
            {
                return _defaultCurrency;
            }

            set
            {
                if (_defaultCurrency == value)
                {
                    return;
                }

                _defaultCurrency = value;
                RaisePropertyChanged(DefaultCurrencyPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                if (LastName == null)
                    return FirstName;
                else
                    return FirstName + " " + LastName;
            }
        }
    }
}
