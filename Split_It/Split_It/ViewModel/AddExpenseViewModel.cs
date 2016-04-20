using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using Split_It.Events;
using Split_It.Model;
using Split_It.Service;
using Split_It.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Split_It.ViewModel
{
    public class AddExpenseViewModel : ViewModelBase
    {
        IDataService _dataService;
        INavigationService _navigationService;
        IDialogService _dialogService;
        IContentDialogService _contentDialogService;

        bool _hasSetPaid, _hasSetSplit;

        public AddExpenseViewModel(IDataService dataService, INavigationService navigationService, IDialogService dialogService, IContentDialogService contentDialogService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _contentDialogService = contentDialogService;
        }

        #region Properties

        /// <summary>
        /// The <see cref="ExpenseToAdd" /> property's name.
        /// </summary>
        public const string ExpenseToAddPropertyName = "ExpenseToAdd";

        private Expense _expenseToAdd = new Expense() { CurrencyCode = AppState.CurrentUser.DefaultCurrency , GroupId = 0};

        /// <summary>
        /// Sets and gets the ExpenseToAdd property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Expense ExpenseToAdd
        {
            get
            {
                return _expenseToAdd;
            }

            set
            {
                if (_expenseToAdd == value)
                {
                    return;
                }

                _expenseToAdd = value;
                RaisePropertyChanged(ExpenseToAddPropertyName);
                if (ExpenseToAdd == null)
                    return;

                if(ExpenseToAdd.Id == 0)
                {
                    _hasSetPaid = false;
                    _hasSetSplit = false;
                }
                else
                {
                    _hasSetPaid = true;
                    _hasSetSplit = true;
                }
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

        #endregion

        #region Commands

        private RelayCommand<IEnumerable<Friend>> _groupSelectedCommand;

        /// <summary>
        /// Gets the GroupSelectedCommand.
        /// </summary>
        public RelayCommand<IEnumerable<Friend>> GroupSelectedCommand
        {
            get
            {
                return _groupSelectedCommand
                    ?? (_groupSelectedCommand = new RelayCommand<IEnumerable<Friend>>(
                    membersList =>
                    {
                        if (membersList == null)
                            return;

                        ObservableCollection<ExpenseUser> users = ExpenseToAdd.Users as ObservableCollection<ExpenseUser>;
                        if (users == null)
                            users = new ObservableCollection<ExpenseUser>();

                        foreach (var item in membersList)
                        {
                            ExpenseUser userToAdd = new ExpenseUser() { User = item, UserId = item.id };
                            if (users.Contains(userToAdd))
                                continue;
                            users.Add(userToAdd);
                        }
                        ExpenseToAdd.Users = users;
                    }));
            }
        }

        private RelayCommand<ExpenseUser> _removeUserCommand;

        /// <summary>
        /// Gets the RemoveUserCommand.
        /// </summary>
        public RelayCommand<ExpenseUser> RemoveUserCommand
        {
            get
            {
                return _removeUserCommand
                    ?? (_removeUserCommand = new RelayCommand<ExpenseUser>(
                    user =>
                    {
                        if (user == null || user.UserId == AppState.CurrentUser.id) //Cannot remove self
                            return;

                        ObservableCollection<ExpenseUser> users = ExpenseToAdd.Users as ObservableCollection<ExpenseUser>;
                        users.Remove(user);
                        ExpenseToAdd.Users = users;
                    }));
            }
        }

        private RelayCommand<ExpenseUser> _addUserCommand;

        /// <summary>
        /// Gets the AddUserCommand.
        /// </summary>
        public RelayCommand<ExpenseUser> AddUserCommand
        {
            get
            {
                return _addUserCommand
                    ?? (_addUserCommand = new RelayCommand<ExpenseUser>(
                    user =>
                    {
                        if (user == null)
                            return;

                        ObservableCollection<ExpenseUser> users = ExpenseToAdd.Users as ObservableCollection<ExpenseUser>;
                        if (users == null)
                        {
                            users = new ObservableCollection<ExpenseUser>();
                            users.Add(new ExpenseUser() { User = AppState.CurrentUser, UserId = AppState.CurrentUser.id });
                        }
                        users.Add(user);
                        ExpenseToAdd.Users = users;
                    }));
            }
        }

        private RelayCommand _whoPaidCommand;

        /// <summary>
        /// Gets the WhoPaidCommand.
        /// </summary>
        public RelayCommand WhoPaidCommand
        {
            get
            {
                return _whoPaidCommand
                    ?? (_whoPaidCommand = new RelayCommand(
                    async () =>
                    {
                        await _contentDialogService.showWhoPaidDialog(ExpenseToAdd);
                        RaisePropertyChanged(ExpenseToAddPropertyName);
                        _hasSetPaid = true;
                    }));
            }
        }

        private RelayCommand _splitCommand;

        /// <summary>
        /// Gets the SplitCommand.
        /// </summary>
        public RelayCommand SplitCommand
        {
            get
            {
                return _splitCommand
                    ?? (_splitCommand = new RelayCommand(
                    async () =>
                    {
                        await _contentDialogService.showSplitDialog(ExpenseToAdd);
                        RaisePropertyChanged(ExpenseToAddPropertyName);
                        _hasSetSplit = true;
                    }));
            }
        }

        private RelayCommand _recordExpenseCommand;

        /// <summary>
        /// Gets the RecordExpenseCommand.
        /// </summary>
        public RelayCommand RecordExpenseCommand
        {
            get
            {
                return _recordExpenseCommand
                    ?? (_recordExpenseCommand = new RelayCommand(
                    () =>
                    {
                        string details = ExpenseToAdd.Details;
                        //MessengerInstance.Send(new ExpenseAddedEvent(returnedExpense));
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

        public override void Cleanup()
        {
            ExpenseToAdd = null;
            ExpenseToAdd = new Expense() { CurrencyCode = AppState.CurrentUser.DefaultCurrency, GroupId = 0 };
            base.Cleanup();
        }
    }
}
