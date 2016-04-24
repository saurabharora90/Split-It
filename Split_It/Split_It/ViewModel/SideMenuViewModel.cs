using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.ViewModel
{
    public class SideMenuViewModel : ViewModelBase
    {
        public SideMenuViewModel()
        {

        }

        /// <summary>
        /// The <see cref="IsAdsEnabled" /> property's name.
        /// </summary>
        public const string IsAdsEnabledPropertyName = "IsAdsEnabled";

        private bool _isAdsEnabled = true;

        /// <summary>
        /// Sets and gets the IsAdsEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsAdsEnabled
        {
            get
            {
                return _isAdsEnabled;
            }

            set
            {
                if (_isAdsEnabled == value)
                {
                    return;
                }

                _isAdsEnabled = value;
                RaisePropertyChanged(IsAdsEnabledPropertyName);
            }
        }

        private RelayCommand _rateAppCommand;

        /// <summary>
        /// Gets the RateAppCommand.
        /// </summary>
        public RelayCommand RateAppCommand
        {
            get
            {
                return _rateAppCommand
                    ?? (_rateAppCommand = new RelayCommand(
                    () =>
                    {

                    }));
            }
        }

        private RelayCommand _removeAdsCommand;

        /// <summary>
        /// Gets the RemoveAdsCommand.
        /// </summary>
        public RelayCommand RemoveAdsCommand
        {
            get
            {
                return _removeAdsCommand
                    ?? (_removeAdsCommand = new RelayCommand(
                    () =>
                    {

                    }));
            }
        }
    }
}
