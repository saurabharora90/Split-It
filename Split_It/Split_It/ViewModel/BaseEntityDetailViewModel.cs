using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Split_It.Events;
using Split_It.Model;
using Split_It.Service;
using System.Collections.ObjectModel;

namespace Split_It.ViewModel
{
    /// <summary>
    /// This is the base class for FriendDetail and GroupsDetail since most of them contain the same functionality.
    /// </summary>
    public abstract class BaseEntityDetailViewModel : ViewModelBase
    {
        protected readonly int _limit = 20;
        protected int _pageNo = 0;

        protected IDataService _dataService;
        protected INavigationService _navigationService;

        public BaseEntityDetailViewModel(IDataService dataService, INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
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

        public Expense SelectedExpense
        {
            set
            {
                if (value !=null)
                {
                    _navigationService.NavigateTo(ViewModelLocator.ExpenseDetailPageKey, value);
                }
            }
        }
        #endregion

        #region Commands
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

        private RelayCommand _registerMessengerCommand;

        /// <summary>
        /// Gets the RegisterMessengerCommand.
        /// </summary>
        public RelayCommand RegisterMessengerCommand
        {
            get
            {
                return _registerMessengerCommand
                    ?? (_registerMessengerCommand = new RelayCommand(
                    () =>
                    {
                        MessengerInstance.Register<ExpenseDeletedEvent>(this, ExpenseDeletedEventReceived);
                        MessengerInstance.Register<ExpenseAddedEvent>(this, ExpenseAddedEventReceived);
                        //MessengerInstance.Register<ExpenseEditedEvent>(this, ExpenseAddedEventReceived);
                    }));
            }
        }

        private RelayCommand _unregisterMessengerCommand;

        /// <summary>
        /// Gets the UnregisterMessengerCommand.
        /// </summary>
        public RelayCommand UnregisterMessengerCommand
        {
            get
            {
                return _unregisterMessengerCommand
                    ?? (_unregisterMessengerCommand = new RelayCommand(
                    () =>
                    {
                        MessengerInstance.Unregister(this);
                    }));
            }
        }
        #endregion

        protected abstract void loadData();

        /// <summary>
        /// Gives an opporunity to the implementing classes to perform specific operations to handle add, delete and edit of expense.
        /// </summary>
        protected abstract void refreshAfterExpenseOperation();

        private void ExpenseDeletedEventReceived(ExpenseDeletedEvent expenseEvent)
        {
            ExpensesList.Remove(expenseEvent.ExpenseChanged);
            refreshAfterExpenseOperation();
        }

        private void ExpenseAddedEventReceived(ExpenseAddedEvent expenseEvent)
        {
            ExpensesList.Insert(0, expenseEvent.ExpenseChanged);
            refreshAfterExpenseOperation();
        }

        public override void Cleanup()
        {
            //base.Cleanup();
        }
    }
}
