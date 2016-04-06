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
                        //TODO: show busy indicator
                        if (url.Contains(Constants.OAUTH_CALLBACK))
                        {
                            var arguments = url.Split('?');
                            if (arguments.Length < 1)
                                return;

                            Tuple<string, string> tuple = await _loginService.getAccessToken(arguments[1]);
                            ServiceLocator.Current.GetInstance<IDataService>().AccessToken = tuple.Item1;
                            ServiceLocator.Current.GetInstance<IDataService>().AccessTokenSecret = tuple.Item2;

                            _navigationService.NavigateTo(ViewModelLocator.MainPageKey);
                        }
                    }));
            }
        }
        
        #endregion
    }
}
