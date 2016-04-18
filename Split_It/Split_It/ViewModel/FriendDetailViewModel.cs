using GalaSoft.MvvmLight.Views;
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
    public class FriendDetailViewModel : BaseEntityDetailViewModel
    {
        public int FriendshipId { get; set; }

        public FriendDetailViewModel(IDataService dataService, INavigationService navigationService, IDialogService dialogService) : base(dataService, navigationService, dialogService)
        {
            init();
        }

        private async void init()
        {
            if (IsInDesignModeStatic)
            {
                List<Friend> list = new List<Friend>(await _dataService.getFriendsList());
                CurrentFriend = list[3];

                RefreshExpensesCommand.Execute(null);
            }
        }

        #region Properties

        /// <summary>
        /// The <see cref="CurrentFriend" /> property's name.
        /// </summary>
        public const string CurrentFriendPropertyName = "CurrentFriend";

        private Friend _currentFriend = null;

        /// <summary>
        /// Sets and gets the CurrentFriend property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Friend CurrentFriend
        {
            get
            {
                return _currentFriend;
            }

            set
            {
                if (_currentFriend == value)
                {
                    return;
                }

                _currentFriend = value;
                RaisePropertyChanged(CurrentFriendPropertyName);

                if (CurrentFriend == null)
                    return;

                if(CurrentFriend.Balance.Count() > 1)
                    CurrentFriend.Balance = CurrentFriend.Balance.Where(p => System.Convert.ToDouble(p.Amount) != 0);
                RefreshExpensesCommand.Execute(null);
            }
        }

        /// <summary>
        /// The <see cref="SettleUpBalance" /> property's name.
        /// </summary>
        public const string SettleUpBalancePropertyName = "SettleUpBalance";

        private UserBalance _settleUpBalance = null;

        /// <summary>
        /// Sets and gets the SettleUpBalance property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public UserBalance SettleUpBalance
        {
            get
            {
                return _settleUpBalance;
            }

            set
            {
                if (_settleUpBalance == value)
                {
                    return;
                }

                _settleUpBalance = value;
                RaisePropertyChanged(SettleUpBalancePropertyName);

                if(SettleUpBalance!=null)
                {
                    IsFlyoutOpen = false;
                    recordPayment();
                }
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
                if (CurrentFriend == null || CurrentFriend.Balance == null)
                    return false;
                else
                {
                    foreach (var balance in CurrentFriend.Balance)
                    {
                        if (System.Convert.ToDouble(balance.Amount) != 0)
                            return true;
                    }

                    return false;
                }
            }
        }

        #endregion

        protected override async void loadData()
        {
            if (_pageNo == 0)
                IsBusy = true;

            var list = await _dataService.getExpenseForFriend(FriendshipId, _limit, _pageNo * _limit);
            list = list.Where(p => p.DeletedAt == null);
            if (_pageNo == 0)
                ExpensesList = new ObservableCollection<Expense>(list);
            else
                ExpensesList.AddRange(list);
            IsBusy = false;
            IsLoadingNewPage = false;
            RaisePropertyChanged(CanSettleUpPropertyName);
        }

        protected async override void refreshAfterExpenseOperation()
        {
            IsBusy = true;
            CurrentFriend.Balance = (await _dataService.getFriendInfo(CurrentFriend.id)).Balance;    //we copy over individual details instead of entire object in order to maintain reference and hence update the item in the main list of the MainViewModel
            IsBusy = false;

            RaisePropertyChanged(CanSettleUpPropertyName);
        }

        public override void Cleanup()
        {
            CurrentFriend = null;
            base.Cleanup();
        }

        private async void recordPayment()
        {
            IsFlyoutOpen = false;
            IsBusy = true;
            double amount = System.Convert.ToDouble(SettleUpBalance.Amount);
            Expense expense = new Expense();
            expense.Payment = true;
            expense.Cost = Math.Abs(amount).ToString();
            expense.CreationMethod = "payment";
            expense.Description = "Payment";
            expense.CurrencyCode = SettleUpBalance.CurrencyCode;

            List<ExpenseUser> expenseUsers = new List<ExpenseUser>(2);
            if (amount > 0)
            {
                expenseUsers.Add(new ExpenseUser { UserId = CurrentFriend.id, PaidShare = expense.Cost, OwedShare = "0" });
                expenseUsers.Add(new ExpenseUser { UserId = AppState.CurrenUserID, OwedShare = expense.Cost, PaidShare = "0" });
            }
            else
            {
                expenseUsers.Add(new ExpenseUser { UserId = AppState.CurrenUserID, PaidShare = expense.Cost, OwedShare = "0" });
                expenseUsers.Add(new ExpenseUser { UserId = CurrentFriend.id, OwedShare = expense.Cost, PaidShare = "0" });
            }

            expense.Users = expenseUsers;

            Expense returnedExpense = (await _dataService.createExpense(expense)).FirstOrDefault();
            if(returnedExpense!=null && returnedExpense.Id!=0)
            {
                ExpensesList.Insert(0, returnedExpense);
                refreshAfterExpenseOperation();
            }
            IsBusy = false;
        }
    }
}
