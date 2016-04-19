using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Split_It.Events;
using Split_It.Model;
using Split_It.Service;
using Split_It.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WinUX.Extensions;

namespace Split_It.ViewModel
{
    public class GroupDetailViewModel : BaseEntityDetailViewModel
    {
        public GroupDetailViewModel(IDataService dataService, INavigationService navigationService, IDialogService dialogService) : base(dataService, navigationService, dialogService)
        {
            init();
        }

        private async void init()
        {
            if (IsInDesignModeStatic)
            {
                List<Group> list = new List<Group>(await _dataService.getGroupsList());
                CurrentGroup = list[0];

                RefreshExpensesCommand.Execute(null);
            }
        }

        protected async override void loadData()
        {
            if (_pageNo == 0)
                IsBusy = true;

            var list = await _dataService.getExpenseForGroup(CurrentGroup.Id, _limit, _pageNo * _limit);
            list = list.Where(p => p.DeletedAt == null);
            if (_pageNo == 0)
                ExpensesList = new ObservableCollection<Expense>(list);
            else
                ExpensesList.AddRange(list);
            IsBusy = false;
            IsLoadingNewPage = false;
        }

        protected async override void refreshAfterExpenseOperation()
        {
            IsBusy = true;
            var group = await _dataService.getGroupInfo(CurrentGroup.Id);
            CurrentGroup.UpdatedAt = group.UpdatedAt;   //we copy over individual details instead of entire object in order to maintain reference and hence update the item in the main list of the MainViewModel
            CurrentGroup.SimplifiedDebts = group.SimplifiedDebts;
            CurrentGroup.OriginalDebts = group.OriginalDebts;
            CurrentGroup.Members = group.Members;
            CurrentGroup.Balance = group.Balance;
            filterBalance();
            RaisePropertyChanged(CanSettleUpPropertyName);
            IsBusy = false;
        }

        private void filterBalance()
        {
            if (CurrentGroup == null)
                return;

            var user = AppState.CurrentUser;
            foreach (var member in CurrentGroup.Members)
            {
                if (member.id == user.id)
                {
                    CurrentUserAsFriend = member;
                    break;
                }
            }

            //Now for current user we have to remove the 0 balances (assuming there are more than one balance.
            if (CurrentUserAsFriend.Balance.Count() > 1)
            {
                CurrentUserAsFriend.Balance = CurrentUserAsFriend.Balance.Where(p => System.Convert.ToDouble(p.Amount) != 0);
            }

            IEnumerable<Debt> allDebts = null;
            if (CurrentGroup.SimplifyByDefault)
                allDebts = CurrentGroup.SimplifiedDebts;
            else
                allDebts = CurrentGroup.OriginalDebts;

            if (UserDebts == null)
                UserDebts = new ObservableCollection<Debt>();
            else
                UserDebts.Clear();

            foreach (var debt in allDebts)
            {
                if (debt.From == CurrentUserAsFriend.id || debt.To == CurrentUserAsFriend.id)
                    UserDebts.Add(debt);
            }
        }

        #region Properties
        /// <summary>
        /// The <see cref="CurrentGroup" /> property's name.
        /// </summary>
        public const string CurrentGroupPropertyName = "CurrentGroup";

        private Group _currentGroup = null;

        /// <summary>
        /// Sets and gets the CurrentGroup property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Group CurrentGroup
        {
            get
            {
                return _currentGroup;
            }

            set
            {
                if (_currentGroup == value)
                {
                    return;
                }

                _currentGroup = value;
                RaisePropertyChanged(CurrentGroupPropertyName);
                if (CurrentGroup == null)
                    return;
                //we cannot use balance for group. we need to use Members for this
                //if (CurrentGroup.Balance.Count() > 1)
                  //  CurrentGroup.Balance = CurrentGroup.Balance.Where(p => System.Convert.ToDouble(p.Amount) != 0);

                if (ExpensesList != null)
                    ExpensesList.Clear();

                RefreshExpensesCommand.Execute(null);

                filterBalance();
            }
        }

        /// <summary>
        /// The <see cref="CurrentUserAsFriend" /> property's name.
        /// </summary>
        public const string CurrentUserAsFriendPropertyName = "CurrentUserAsFriend";

        private Friend _currentUserAsFriend = null;

        /// <summary>
        /// Sets and gets the CurrentUserAsFriend property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Friend CurrentUserAsFriend
        {
            get
            {
                return _currentUserAsFriend;
            }

            set
            {
                if (_currentUserAsFriend == value)
                {
                    return;
                }

                _currentUserAsFriend = value;
                RaisePropertyChanged(CurrentUserAsFriendPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="UserDebts" /> property's name.
        /// </summary>
        public const string UserDebtsPropertyName = "UserDebts";

        private ObservableCollection<Debt> _userDebts = null;

        /// <summary>
        /// Sets and gets the UserDebts property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Debt> UserDebts
        {
            get
            {
                return _userDebts;
            }

            set
            {
                if (_userDebts == value)
                {
                    return;
                }

                _userDebts = value;

                RaisePropertyChanged(UserDebtsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsFlyoutOpen" /> property's name.
        /// </summary>
        public const string IsFlyoutOpenPropertyName = "IsFlyoutOpen";

        private bool _isFlyoutOpen = false;

        /// <summary>
        /// Sets and gets the IsFlyoutOpen property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsFlyoutOpen
        {
            get
            {
                return _isFlyoutOpen;
            }

            set
            {
                if (_isFlyoutOpen == value)
                {
                    return;
                }

                _isFlyoutOpen = value;
                RaisePropertyChanged(IsFlyoutOpenPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CanSettleUp" /> property's name.
        /// </summary>
        public const string CanSettleUpPropertyName = "CanSettleUp";

        public bool CanSettleUp
        {
            get
            {
                if (CurrentUserAsFriend.Balance == null)
                    return false;
                else
                {
                    foreach (var balance in CurrentUserAsFriend.Balance)
                    {
                        if (System.Convert.ToDouble(balance.Amount) != 0)
                            return true;
                    }

                    return false;
                }
            }
        }

        public Debt DebtToSettle
        {
            set
            {
                if (value != null)
                {
                    recordPayment(value);
                }
            }
        }

        #endregion

        private RelayCommand _addExpenseCommand;

        /// <summary>
        /// Gets the AddExpenseCommand.
        /// </summary>
        public RelayCommand AddExpenseCommand
        {
            get
            {
                return _addExpenseCommand
                    ?? (_addExpenseCommand = new RelayCommand(
                    () =>
                    {
                        var expense = new Expense() { CurrencyCode = AppState.CurrentUser.DefaultCurrency, GroupId = CurrentGroup.Id };
                        ObservableCollection<ExpenseUser> users = new ObservableCollection<ExpenseUser>();
                        foreach (var item in CurrentGroup.Members)
                        {
                            ExpenseUser userToAdd = new ExpenseUser() { User = item, UserId = item.id };
                            users.Add(userToAdd);
                        }
                        expense.Users = users;
                        _navigationService.NavigateTo(ViewModelLocator.AddExpensePageKey, expense);
                    }));
            }
        }

        private async void recordPayment(Debt debtToSettle)
        {
            IsFlyoutOpen = false;
            IsBusy = true;
            double amount = System.Convert.ToDouble(debtToSettle.Amount);
            Expense expense = new Expense();
            expense.Payment = true;
            expense.Cost = Math.Abs(amount).ToString();
            expense.CreationMethod = "payment";
            expense.Description = "Payment";
            expense.CurrencyCode = debtToSettle.CurrencyCode;
            expense.GroupId = CurrentGroup.Id;

            List<ExpenseUser> expenseUsers = new List<ExpenseUser>(2);
            expenseUsers.Add(new ExpenseUser { UserId = debtToSettle.From, PaidShare = expense.Cost, OwedShare = "0" });
            expenseUsers.Add(new ExpenseUser { UserId = debtToSettle.To, OwedShare = expense.Cost, PaidShare = "0" });
            
            expense.Users = expenseUsers;

            Expense returnedExpense = (await _dataService.createExpense(expense)).FirstOrDefault();
            if (returnedExpense != null && returnedExpense.Id != 0)
            {
                MessengerInstance.Send(new ExpenseAddedEvent(returnedExpense));
            }
            IsBusy = false;
        }

        public override void Cleanup()
        {
            CurrentUserAsFriend = null;
            CurrentGroup = null;
            if(UserDebts!=null)
            {
                UserDebts.Clear();
                UserDebts = null;
            }
            base.Cleanup();
        }
        
    }
}
