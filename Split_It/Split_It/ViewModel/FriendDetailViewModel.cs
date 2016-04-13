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

                if (ExpensesList!=null)
                    ExpensesList.Clear();

                RefreshExpensesCommand.Execute(null);
            }
        }

        /// <summary>
        /// The <see cref="ExpensesList" /> property's name.
        /// </summary>
        public const string ExpensesListPropertyName = "ExpensesList";

        private ObservableCollection<Expense> _expensesList = null;

        /// <summary>
        /// Sets and gets the ExpensesList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Expense> ExpensesList
        {
            get
            {
                return _expensesList;
            }

            set
            {
                if (_expensesList == value)
                {
                    return;
                }

                _expensesList = value;
                RaisePropertyChanged(ExpensesListPropertyName);
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

        protected override void handleExpenseSelection()
        {
            if(SelectedExpense!=null) 
                _navigationService.NavigateTo(ViewModelLocator.ExpenseDetailPageKey, ExpenseDetailPage.TYPE_FRIEND);
            base.handleExpenseSelection();
        }

        protected override void expenseUpdated()
        {
            base.expenseUpdated();
            _navigationService.GoBack();
            RefreshExpensesCommand.Execute(null);
        }
    }
}
