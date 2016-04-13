using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Split_It.Model;
using Split_It.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.ViewModel
{
    public class ExpenseDetailViewModel : ViewModelBase
    {
        protected IDataService _dataService;
        protected INavigationService _navigationService;

        public ExpenseDetailViewModel(IDataService dataService, INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
        }

        /// <summary>
        /// The <see cref="SelectedExpense" /> property's name.
        /// </summary>
        public const string SelectedExpensePropertyName = "SelectedExpense";

        private Expense _selectedExpense = null;

        /// <summary>
        /// Sets and gets the SelectedExpense property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Expense SelectedExpense
        {
            get
            {
                return _selectedExpense;
            }

            set
            {
                if (_selectedExpense == value)
                {
                    return;
                }

                _selectedExpense = value;
                RaisePropertyChanged(SelectedExpensePropertyName);
                handleExpenseSelection();
            }
        }

        #region Commands
        private RelayCommand _deleteCommand;

        /// <summary>
        /// Gets the DeleteCommand.
        /// </summary>
        public RelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand
                    ?? (_deleteCommand = new RelayCommand(
                    () =>
                    {

                    }));
            }
        }

        private RelayCommand _editCommand;

        /// <summary>
        /// Gets the EditCommand.
        /// </summary>
        public RelayCommand EditCommand
        {
            get
            {
                return _editCommand
                    ?? (_editCommand = new RelayCommand(
                    () =>
                    {

                    }));
            }
        }

        private RelayCommand _addCommentCommand;

        /// <summary>
        /// Gets the AddCommentCommand.
        /// </summary>
        public RelayCommand AddCommentCommand
        {
            get
            {
                return _addCommentCommand
                    ?? (_addCommentCommand = new RelayCommand(
                    () =>
                    {

                    }));
            }
        }
        #endregion

        protected virtual void handleExpenseSelection()
        {

        }
    }
}
