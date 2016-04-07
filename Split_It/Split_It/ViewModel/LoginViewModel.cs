using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Split_It.Service;
using Split_It.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private IWebviewService _webviewService;
        private ILoginService _loginService;
        private INavigationService _navigationService;

        public LoginViewModel(IWebviewService webService, ILoginService loginService, INavigationService navigationService)
        {
            _webviewService = webService;
            _loginService = loginService;
            _navigationService = navigationService;

            init();
        }

        private async void init()
        {
            _webviewService.NavigateTo(await _loginService.getRequestToken());

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

        #region Commands

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
                    url =>
                    {
                        IsBusy = false;
                    }));
            }
        }

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
                    async url =>
                    {
                        IsBusy = true;
                        if (url.Contains(Constants.OAUTH_CALLBACK))
                        {
                            var arguments = url.Split('?');
                            if (arguments.Length < 1)
                                return;

                            Tuple<string, string> tuple = await _loginService.getAccessToken(arguments[1]);
                            ServiceLocator.Current.GetInstance<IDataService>().AccessToken = tuple.Item1;
                            ServiceLocator.Current.GetInstance<IDataService>().AccessTokenSecret = tuple.Item2;
                            IsBusy = false;
                            _navigationService.NavigateTo(ViewModelLocator.MainPageKey);
                        }
                    }));
            }
        }
        
        #endregion
    }
}
