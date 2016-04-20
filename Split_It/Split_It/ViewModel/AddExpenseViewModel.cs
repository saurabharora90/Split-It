using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using Split_It.Events;
using Split_It.Model;
using Split_It.Model.Enum;
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

                setupExpenseSplit();
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
        /// The <see cref="SplitType" /> property's name.
        /// </summary>
        public const string SplitTypePropertyName = "SplitType";

        private ExpenseSplit _splitType = ExpenseSplit.EQUALLY;

        /// <summary>
        /// Sets and gets the SplitType property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ExpenseSplit SplitType
        {
            get
            {
                return _splitType;
            }

            set
            {
                if (_splitType == value)
                {
                    return;
                }

                _splitType = value;
                RaisePropertyChanged(SplitTypePropertyName);
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

                        if (users.Contains(user))
                            return;

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
                        if (ExpenseToAdd.Users.Count() == 2)
                            await _contentDialogService.showIOUDialog(ExpenseToAdd);
                        else
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
                    async () =>
                    {
                        if (ExpenseToAdd.Users == null || ExpenseToAdd.Users.Count() < 2 || String.IsNullOrEmpty(ExpenseToAdd.Description) || String.IsNullOrEmpty(ExpenseToAdd.Cost))
                        {
                            await _dialogService.ShowMessage("Please enter all the details", "Oops");
                            return;
                        }

                        IsBusy = true;
                        if (!_hasSetPaid)
                        {
                            foreach (var user in ExpenseToAdd.Users)
                            {
                                if (user.UserId == AppState.CurrentUser.id)
                                {
                                    user.PaidShare = ExpenseToAdd.Cost;
                                    break;
                                }
                            }
                        }

                        if (!_hasSetSplit)
                        {
                            decimal eachPersonAmount = Math.Round(Convert.ToDecimal(ExpenseToAdd.Cost) / ExpenseToAdd.Users.Count(), 2);
                            decimal amountLeftOver = Convert.ToDecimal(ExpenseToAdd.Cost) - (eachPersonAmount * ExpenseToAdd.Users.Count());
                            foreach (var item in ExpenseToAdd.Users)
                            {
                                item.OwedShare = eachPersonAmount.ToString();
                            }
                            //add the left over amount to the first person
                            if(amountLeftOver != 0)
                            {
                                var enumerator = ExpenseToAdd.Users.GetEnumerator();
                                enumerator.MoveNext();
                                var user = enumerator.Current;
                                decimal currentAmount = Convert.ToDecimal(user.OwedShare);
                                decimal finalAmount = currentAmount + amountLeftOver;
                                user.OwedShare = finalAmount.ToString();
                            }
                        }

                        bool isAdd = ExpenseToAdd.Id == 0 ? true : false;
                        Expense returnedExpense = (await _dataService.createExpense(ExpenseToAdd)).FirstOrDefault();
                        if (isAdd && returnedExpense != null && returnedExpense.Id != 0)
                        {
                            MessengerInstance.Send(new ExpenseAddedEvent(returnedExpense));
                            _navigationService.GoBack();
                        }
                        else if (!isAdd && returnedExpense != null)
                        {
                            MessengerInstance.Send(new ExpenseEditedEvent(returnedExpense));
                            _navigationService.GoBack();
                        }

                        IsBusy = false;
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

        private void setupExpenseSplit()
        {
            if (ExpenseToAdd == null || System.Convert.ToDouble(ExpenseToAdd.Cost) == 0 || ExpenseToAdd.Users == null || ExpenseToAdd.Users.Count() < 2 || ExpenseToAdd.Id == 0)
                SplitType = ExpenseSplit.EQUALLY;
            else
            {
                if(ExpenseToAdd.Users.Count() == 2)
                {
                    //Here we check if it is ExpenseSplit.THEY_OWE or ExpenseSplit.YOU_OWE
                    var enumarator = ExpenseToAdd.Users.GetEnumerator();
                    enumarator.MoveNext();
                    var user = enumarator.Current;
                    enumarator.Dispose();
                    if (System.Convert.ToDouble(user.OwedShare) == 0)
                    {
                        if(user.UserId == AppState.CurrentUser.id)
                        {
                            SplitType = ExpenseSplit.THEY_OWE;
                            return;
                        }
                        else
                        {
                            SplitType = ExpenseSplit.YOU_OWE;
                            return;
                        }
                    }
                }

                SplitType = ExpenseSplit.UNEQUALLY; //To edit expense with more than 2 users, we just default to unequal. Makes life easier
            }
        }

        public override void Cleanup()
        {
            ExpenseToAdd = null;
            ExpenseToAdd = new Expense() { CurrencyCode = AppState.CurrentUser.DefaultCurrency, GroupId = 0 };        
            base.Cleanup();
        }
    }
}
