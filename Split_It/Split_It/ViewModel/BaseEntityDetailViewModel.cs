using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Split_It.Service;

namespace Split_It.ViewModel
{
    /// <summary>
    /// This is the base class for friendDetail and GroupsDetail since most of them contain the same functionality.
    /// </summary>
    public abstract class BaseEntityDetailViewModel : ExpenseDetailViewModel
    {

        protected readonly int _limit = 20;
        protected int _pageNo = 0;

        public BaseEntityDetailViewModel(IDataService dataService, INavigationService navigationService, IDialogService dialogService) : base(dataService, navigationService, dialogService)
        {

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
        #endregion

        protected abstract void loadData();
    }
}
