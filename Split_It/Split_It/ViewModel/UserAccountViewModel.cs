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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.ViewModel
{
    public class UserAccountViewModel : ViewModelBase
    {
        IDataService _dataService;
        INavigationService _navigationService;
        IDialogService _dialogService;

        public UserAccountViewModel(IDataService dataService, INavigationService navigationService, IDialogService dialogService)
        {
            this._dataService = dataService;
            this._navigationService = navigationService;
            this._dialogService = dialogService;
        }

        /// <summary>
        /// The <see cref="CurrentUser" /> property's name.
        /// </summary>
        public const string CurrentUserPropertyName = "CurrentUser";

        private User _currentUser = null;

        /// <summary>
        /// Sets and gets the CurrentUser property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return _currentUser;
            }

            set
            {
                if (_currentUser == value)
                {
                    return;
                }

                _currentUser = value;
                RaisePropertyChanged(CurrentUserPropertyName);
            }
        }

        public ObservableCollection<Currency> SupportedCurrencyList
        {
            get
            {
                string text = File.ReadAllText("Data/currencies.json");
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(text);
                Newtonsoft.Json.Linq.JToken testToken = root["currencies"];
                return new ObservableCollection<Currency>(JsonConvert.DeserializeObject<IEnumerable<Currency>>(testToken.ToString()).OrderBy(p => p.CurrencyCode));
            }
        }

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy = false;

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
                    async () =>
                    {
                        IsBusy = true;
                        User user = await _dataService.updateCurrentUser(CurrentUser);
                        IsBusy = false;
                        if (user.id != -1)
                        {
                            AppState.CurrentUser.FirstName = user.FirstName;
                            AppState.CurrentUser.LastName = user.LastName;
                            AppState.CurrentUser.Email = user.Email;
                            AppState.CurrentUser.DefaultCurrency = user.DefaultCurrency;

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
