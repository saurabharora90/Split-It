using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Split_It.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.ViewModel
{
    public class UserAccountViewModel : ViewModelBase
    {
        IDataService _dataService;
        INavigationService _navigationService;

        public UserAccountViewModel(IDataService dataService, INavigationService navigationService)
        {
            this._dataService = dataService;
            this._navigationService = navigationService;
        }

        #region Commands

        private RelayCommand _saveCommand;

        /// <summary>
        /// Gets the SaveCommand.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand
                    ?? (_saveCommand = new RelayCommand(
                    () =>
                    {

                    }));
            }
        }

        private RelayCommand _goBackCommand;

        /// <summary>
        /// Gets the GoBackCommand.
        /// </summary>
        public RelayCommand GoBackCommand
        {
            get
            {
                return _goBackCommand
                    ?? (_goBackCommand = new RelayCommand(
                    () =>
                    {
                        _navigationService.GoBack();
                    }));
            }
        }

        #endregion
    }
}
