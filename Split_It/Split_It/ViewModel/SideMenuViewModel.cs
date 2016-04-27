using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Split_It.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.System;

namespace Split_It.ViewModel
{
    public class SideMenuViewModel : ViewModelBase
    {
        LicenseInformation licenseInformation;

        public SideMenuViewModel()
        {
#if DEBUG
            licenseInformation = CurrentAppSimulator.LicenseInformation;
#else
            licenseInformation = CurrentApp.LicenseInformation;
#endif
            if (licenseInformation.ProductLicenses[Constants.REMOVE_ADS_NEW_PRODUCT_ID].IsActive || licenseInformation.ProductLicenses[Constants.REMOVE_ADS_OLD_PRODUCT_ID].IsActive)
            {
                IsAdsEnabled = false;
            }
            else
            {
                IsAdsEnabled = true;
            }
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
                    async () =>
                    {
                        await Launcher.LaunchUriAsync(new Uri(string.Format("ms-windows-store:REVIEW?PFN={0}", Windows.ApplicationModel.Package.Current.Id.FamilyName)));
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
                    async () =>
                    {
                        try
                        {
                            // The customer doesn't own this feature, so 
                            // show the purchase dialog.

                            string code = await CurrentAppSimulator.RequestProductPurchaseAsync(Constants.REMOVE_ADS_NEW_PRODUCT_ID, false);
                            //Check the license state to determine if the in-app purchase was successful.
                            if (licenseInformation.ProductLicenses[Constants.REMOVE_ADS_NEW_PRODUCT_ID].IsActive)
                            {
                                IsAdsEnabled = false;
                            }
                        }
                        catch (Exception e)
                        {
                            // The in-app purchase was not completed because 
                            // an error occurred.
                            Debug.Write(e);
                        }
                    }));
            }
        }
    }
}
