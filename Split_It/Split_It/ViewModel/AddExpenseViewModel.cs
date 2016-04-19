using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Split_It.Model;
using Split_It.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.ViewModel
{
    public class AddExpenseViewModel : ViewModelBase
    {
        IDataService _dataService;
        INavigationService _navigationService;
        IDialogService _dialogService;

        public AddExpenseViewModel(IDataService dataService, INavigationService navigationService, IDialogService dialogService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
            _dialogService = dialogService;
        }

        #region Properties

        /// <summary>
        /// The <see cref="ExpenseToAdd" /> property's name.
        /// </summary>
        public const string ExpenseToAddPropertyName = "ExpenseToAdd";

        private Expense _expenseToAdd = null;

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
            }
        }

        /// <summary>
        /// The <see cref="SelectedGroup" /> property's name.
        /// </summary>
        public const string SelectedGroupPropertyName = "SelectedGroup";

        private Group _selectedGroup = null;

        /// <summary>
        /// Sets and gets the SelectedGroup property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Group SelectedGroup
        {
            get
            {
                return _selectedGroup;
            }

            set
            {
                if (_selectedGroup == value)
                {
                    return;
                }

                _selectedGroup = value;
                RaisePropertyChanged(SelectedGroupPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SelectedFriendsList" /> property's name.
        /// </summary>
        public const string SelectedFriendsListPropertyName = "SelectedFriendsList";

        private ObservableCollection<Friend> _selectedFriendsList = null;

        /// <summary>
        /// Sets and gets the SelectedFriendsList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Friend> SelectedFriendsList
        {
            get
            {
                return _selectedFriendsList;
            }

            set
            {
                if (_selectedFriendsList == value)
                {
                    return;
                }

                _selectedFriendsList = value;
                RaisePropertyChanged(SelectedFriendsListPropertyName);
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

        #endregion

        #region Commands

        private RelayCommand _recordExpense;

        /// <summary>
        /// Gets the RecordExpense.
        /// </summary>
        public RelayCommand RecordExpense
        {
            get
            {
                return _recordExpense
                    ?? (_recordExpense = new RelayCommand(
                    () =>
                    {

                    }));
            }
        }

        #endregion

        public override void Cleanup()
        {
            if (SelectedFriendsList != null)
                SelectedFriendsList.Clear();
            SelectedFriendsList = null;
            SelectedGroup = null;
            ExpenseToAdd = null;
            base.Cleanup();
        }
    }
}
