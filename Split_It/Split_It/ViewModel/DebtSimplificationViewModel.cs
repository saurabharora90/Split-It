using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using Split_It.Model;
using Split_It.Service;
using Split_It.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.ViewModel
{
    public class DebtSimplificationViewModel : ViewModelBase
    {
        INavigationService _navigationService;
        IDialogService _dialogService;

        public DebtSimplificationViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            this._navigationService = navigationService;
            this._dialogService = dialogService;
        }

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy = true;

        /// <summary>
        /// Sets and gets the IsBusy property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                if (_isBusy == value)
                {
                    return;
                }

                _isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);
            }
        }

        #region 

        private RelayCommand<string> _webviewNavigatingCommand;

        /// <summary>
        /// Gets the WebviewNavigatingCommand.
        /// </summary>
        public RelayCommand<string> WebviewNavigatingCommand
        {
            get
            {
                return _webviewNavigatingCommand
                    ?? (_webviewNavigatingCommand = new RelayCommand<string>(
                    url =>
                    {
                        IsBusy = true;
                    }));
            }
        }

        private RelayCommand<string> _webviewNavigatedCommand;

        /// <summary>
        /// Gets the WebviewNavigatedCommand.
        /// </summary>
        public RelayCommand<string> WebviewNavigatedCommand
        {
            get
            {
                return _webviewNavigatedCommand
                    ?? (_webviewNavigatedCommand = new RelayCommand<string>(
                    async url =>
                    {
                        IsBusy = false;
                        if (url.Contains("edit"))
                        {
                            await _dialogService.ShowMessage("You might need to restart/refresh the app for the changes to reflect", "Success");
                            _navigationService.GoBack();
                        }
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
