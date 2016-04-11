using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Model
{
    public class Photo : GalaSoft.MvvmLight.ObservableObject
    {
        /// <summary>
        /// The <see cref="Small" /> property's name.
        /// </summary>
        public const string SmallPropertyName = "Small";

        private string _small = null;

        /// <summary>
        /// Sets and gets the Small property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        /// 
        public string Small
        {
            get
            {
                return _small;
            }

            set
            {
                if (_small == value)
                {
                    return;
                }

                _small = value;
                RaisePropertyChanged(SmallPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Medium" /> property's name.
        /// </summary>
        public const string MediumPropertyName = "Medium";

        private string _medium = null;

        /// <summary>
        /// Sets and gets the Medium property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Medium
        {
            get
            {
                return _medium;
            }

            set
            {
                if (_medium == value)
                {
                    return;
                }

                _medium = value;
                RaisePropertyChanged(MediumPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Large" /> property's name.
        /// </summary>
        public const string LargePropertyName = "Large";

        private string _large = null;

        /// <summary>
        /// Sets and gets the Large property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Large
        {
            get
            {
                return _large;
            }

            set
            {
                if (_large == value)
                {
                    return;
                }

                _large = value;
                RaisePropertyChanged(LargePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Original" /> property's name.
        /// </summary>
        public const string OriginalPropertyName = "Original";

        private string _original = null;

        /// <summary>
        /// Sets and gets the Original property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Original
        {
            get
            {
                return _original;
            }

            set
            {
                if (_original == value)
                {
                    return;
                }

                _original = value;
                RaisePropertyChanged(OriginalPropertyName);
            }
        }
    }
}
