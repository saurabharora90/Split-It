using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Split_It.Model;
using Split_It.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WinUX.Extensions;

namespace Split_It.ViewModel
{
    public class GroupDetailViewModel : BaseEntityDetailViewModel
    {
        public GroupDetailViewModel(IDataService dataService, INavigationService navigationService) : base(dataService, navigationService)
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
            IsBusy = false;
            filterBalance();
        }

        private void filterBalance()
        {
            if (CurrentGroup == null)
                return;
            Task.Factory.StartNew(() =>
            {
                var user = ServiceLocator.Current.GetInstance<MainViewModel>().CurrentUser;
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
            });
        }

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
                DispatcherHelper.CheckBeginInvokeOnUI(() => { RaisePropertyChanged(CurrentUserAsFriendPropertyName); });
                //RaisePropertyChanged(CurrentUserAsFriendPropertyName);
            }
        }

        public override void Cleanup()
        {
            CurrentUserAsFriend = null;
            CurrentGroup = null;
            base.Cleanup();
        }
    }
}
