using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Split_It.Events;
using Split_It.Model;
using Split_It.Service;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WinUX.Extensions;
using System.Linq;
using System;

namespace Split_It.ViewModel
{
    public class ExpenseDetailViewModel : ViewModelBase
    {
        protected IDataService _dataService;
        protected INavigationService _navigationService;
        protected IDialogService _dialogService;

        public ExpenseDetailViewModel(IDataService dataService, INavigationService navigationService, IDialogService dialogService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            init();
        }

        private async void init()
        {
            if (IsInDesignModeStatic)
            {
                List<Expense> list = new List<Expense>(await _dataService.getExpenseForFriend(-1, 20));
                SelectedExpense = list[4];
            }
        }

        private async void fetchComments()
        {
            var list = await _dataService.getComments(SelectedExpense.Id);
            list = list.Where(p => (p.DeletedAt == String.Empty) || (p.DeletedAt == null));
            if (CommentsList == null)
                CommentsList = new ObservableCollection<Comment>(list);
            else
                CommentsList.AddRange(list);
        }

        #region Properties

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
                if(value!=null)
                {
                    if (CommentsList != null)
                        CommentsList.Clear();

                    if (SelectedExpense.CommentsCount > 0)
                    {
                        fetchComments();
                    }
                }
            }
        }

        /// <summary>
        /// The <see cref="CommentsList" /> property's name.
        /// </summary>
        public const string CommentsListPropertyName = "CommentsList";

        private ObservableCollection<Comment> _commentList = null;

        /// <summary>
        /// Sets and gets the CommentsList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Comment> CommentsList
        {
            get
            {
                return _commentList;
            }

            set
            {
                if (_commentList == value)
                {
                    return;
                }

                _commentList = value;
                RaisePropertyChanged(CommentsListPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy = false;

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
                    async () =>
                    {
                        IsBusy = true;
                        bool result = await _dataService.deleteExpense(SelectedExpense.Id);
                        IsBusy = false;
                        if (result)
                        {
                            MessengerInstance.Send(new ExpenseDeletedEvent(SelectedExpense));
                            _navigationService.GoBack();
                        }
                        else
                            await _dialogService.ShowMessage("Unable to delete expense", "Error");
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

        public override void Cleanup()
        {
            SelectedExpense = null;

            if(CommentsList!=null)
                CommentsList.Clear();

            CommentsList = null;
            base.Cleanup();
        }
    }
}
