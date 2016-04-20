using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Split_It.Model;

namespace Split_It.ViewModel
{
    public class WhoPaidDialogViewModel : ViewModelBase
    {
        public WhoPaidDialogViewModel()
        {

        }

        #region Properties
        /// <summary>
        /// The <see cref="CurrentExpense" /> property's name.
        /// </summary>
        public const string CurrentExpensePropertyName = "CurrentExpense";

        private Expense _currentExpense = null;

        /// <summary>
        /// Sets and gets the CurrentExpense property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Expense CurrentExpense
        {
            get
            {
                return _currentExpense;
            }

            set
            {
                MultiplePeopleSelected = false;
                if (_currentExpense == value)
                {
                    return;
                }

                _currentExpense = value;
                RaisePropertyChanged(CurrentExpensePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="MultiplePeopleSelected" /> property's name.
        /// </summary>
        public const string MultiplePeopleSelectedPropertyName = "MultiplePeopleSelected";

        private bool _multiplePeopleSelected = false;

        /// <summary>
        /// Sets and gets the MultiplePeopleSelected property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool MultiplePeopleSelected
        {
            get
            {
                return _multiplePeopleSelected;
            }

            set
            {
                if (_multiplePeopleSelected == value)
                {
                    return;
                }

                _multiplePeopleSelected = value;
                RaisePropertyChanged(MultiplePeopleSelectedPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CanExit" /> property's name.
        /// </summary>
        public const string CanExitPropertyName = "CanExit";

        private bool _canExit = false;

        /// <summary>
        /// Sets and gets the CanExit property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool CanExit
        {
            get
            {
                return _canExit;
            }

            set
            {
                if (_canExit == value)
                {
                    return;
                }

                _canExit = value;
                RaisePropertyChanged(CanExitPropertyName);
            }
        }
        
        public ExpenseUser SelectedUser
        {
            set
            {
                foreach (var item in CurrentExpense.Users)
                    item.PaidShare = "0.0";
                
                value.PaidShare = CurrentExpense.Cost;
            }
        }

        #endregion

        #region Commands
        private RelayCommand _multiplePeopleSelectedCommand;

        /// <summary>
        /// Gets the MultiplePeopleSelectedCommand.
        /// </summary>
        public RelayCommand MultiplePeopleSelectedCommand
        {
            get
            {
                return _multiplePeopleSelectedCommand
                    ?? (_multiplePeopleSelectedCommand = new RelayCommand(
                    () =>
                    {
                        MultiplePeopleSelected = true;
                    }));
            }
        }
        #endregion
    }
}
