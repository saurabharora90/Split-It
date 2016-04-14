using GalaSoft.MvvmLight.Views;
using Split_It.Model;
using Split_It.Service;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WinUX.Extensions;

namespace Split_It.ViewModel
{
    public class FriendDetailViewModel : BaseEntityDetailViewModel
    {
        public int FriendshipId { get; set; }

        public FriendDetailViewModel(IDataService dataService, INavigationService navigationService) : base(dataService, navigationService)
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
        }

        protected async override void refreshAfterExpenseOperation()
        {
            IsBusy = true;
            CurrentFriend.Balance = (await _dataService.getFriendInfo(CurrentFriend.id)).Balance;    //we copy over individual details instead of entire object in order to maintain reference and hence update the item in the main list of the MainViewModel
            IsBusy = false;
        }

        public override void Cleanup()
        {
            CurrentFriend = null;
            base.Cleanup();
        }
    }
}
