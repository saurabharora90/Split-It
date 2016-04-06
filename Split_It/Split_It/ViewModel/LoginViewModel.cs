using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Split_It.Service;
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

        public LoginViewModel(IWebviewService webService, ILoginService loginService)
        {
            _webviewService = webService;
            _loginService = loginService;
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
                    url =>
                    {
                        
                    }));
            }
        }
    }
}
