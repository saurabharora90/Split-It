using GalaSoft.MvvmLight.Views;
using Split_It.Model;
using Split_It.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

            var list = await _dataService.getExpenseForFriend(CurrentGroup.GroupId, _limit, _pageNo * _limit);
            list = list.Where(p => p.DeletedAt == null);
            if (_pageNo == 0)
                ExpensesList = new ObservableCollection<Expense>(list);
            else
                ExpensesList.AddRange(list);
            IsBusy = false;
            IsLoadingNewPage = false;
        }

        protected override void refreshAfterExpenseOperation()
        {
            throw new NotImplementedException();
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

                if (CurrentGroup.Balance.Count() > 1)
                    CurrentGroup.Balance = CurrentGroup.Balance.Where(p => System.Convert.ToDouble(p.Amount) != 0);

                if (ExpensesList != null)
                    ExpensesList.Clear();

                RefreshExpensesCommand.Execute(null);
            }
        }
    }
}
