using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Split_It.Model;
using Split_It.Service;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WinUX.Extensions;

namespace Split_It.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase
    {
        private readonly int _limit = 20;
        private int _pageNo = 0;
        IDataService _dataService;
        INavigationService _navigationService;
        public int FriendshipId { get; set; }

        public FriendDetailViewModel(IDataService dataService, INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;

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
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy = true;

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

        /// <summary>
        /// The <see cref="IsLoadingNewPage" /> property's name.
        /// </summary>
        public const string IsLoadingNewPagePropertyName = "IsLoadingNewPage";

        private bool _isLoadingNewPage = false;

        /// <summary>
        /// Sets and gets the IsLoadingNewPage property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsLoadingNewPage
        {
            get
            {
                return _isLoadingNewPage;
            }

            set
            {
                if (_isLoadingNewPage == value)
                {
                    return;
                }

                _isLoadingNewPage = value;
                RaisePropertyChanged(IsLoadingNewPagePropertyName);
            }
        }

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

        #region Commands
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

        private RelayCommand _refeshExpensesCommand;

        /// <summary>
        /// Gets the RefreshExpensesCommand.
        /// </summary>
        public RelayCommand RefreshExpensesCommand
        {
            get
            {
                return _refeshExpensesCommand
                    ?? (_refeshExpensesCommand = new RelayCommand(
                    () =>
                    {
                        _pageNo = 0;
                        loadData();
                    }));
            }
        }

        private RelayCommand _loadMoreCommand;

        /// <summary>
        /// Gets the LoadMoreCommand.
        /// </summary>
        public RelayCommand LoadMoreCommand
        {
            get
            {
                return _loadMoreCommand
                    ?? (_loadMoreCommand = new RelayCommand(
                    () =>
                    {
                        IsLoadingNewPage = true;
                        _pageNo++;
                        loadData();
                    }));
            }
        }
        #endregion

        private async void loadData()
        {
            if (_pageNo == 0)
                IsBusy = true;

            var list = await _dataService.getExpenseForFriend(FriendshipId, _limit, _pageNo * _limit);
            if (_pageNo == 0)
                ExpensesList = new ObservableCollection<Expense>(list);
            else
                ExpensesList.AddRange(list);
            IsBusy = false;
            IsLoadingNewPage = false;
        }
    }
}
