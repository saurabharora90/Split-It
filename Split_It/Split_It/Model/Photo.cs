namespace Split_It.Model
{
    /// <summary>
    /// We don't need the samll and large photos. Uselessly taking up memory.
    /// </summary>
    public class Photo : GalaSoft.MvvmLight.ObservableObject
    {

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
