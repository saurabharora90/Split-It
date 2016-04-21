using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Split_It.Model;
using Split_It.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Split_It.ViewModel
{
    public class SplitDialogViewModel : ViewModelBase
    {
        public List<ExpenseSplit> SplitOptions { get; private set; }

        public SplitDialogViewModel()
        {
            SplitOptions = new List<ExpenseSplit>(3);
            SplitOptions.Add(ExpenseSplit.UNEQUALLY);
            SplitOptions.Add(ExpenseSplit.SHARES);
            SplitOptions.Add(ExpenseSplit.EQUALLY);
        }

        #region Properties
        /// <summary>
        /// The <see cref="CurrentExpense" /> property's name.
        /// </summary>
        public const string CurrentExpensePropertyName = "CurrentExpense";

        private Expense _curentExpense = null;

        /// <summary>
        /// Sets and gets the CurrentExpense property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Expense CurrentExpense
        {
            get
            {
                return _curentExpense;
            }

            set
            {
                if (_curentExpense == value)
                {
                    return;
                }

                _curentExpense = value;
                RaisePropertyChanged(CurrentExpensePropertyName);
                SelectedSplitOption = ExpenseSplit.UNEQUALLY;
            }
        }

        /// <summary>
        /// The <see cref="SelectedSplitOption" /> property's name.
        /// </summary>
        public const string SelectedSplitOptionPropertyName = "SelectedSplitOption";

        private ExpenseSplit _selectedSplitOption = ExpenseSplit.UNEQUALLY;

        /// <summary>
        /// Sets and gets the SelectedSplitOption property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ExpenseSplit SelectedSplitOption
        {
            get
            {
                return _selectedSplitOption;
            }

            set
            {
                if (_selectedSplitOption == value)
                {
                    return;
                }

                _selectedSplitOption = value;
                RaisePropertyChanged(SelectedSplitOptionPropertyName);                
            }
        }
        #endregion

        private RelayCommand _primaryButtonCommand;

        /// <summary>
        /// Gets the PrimaryButtonCommand.
        /// </summary>
        public RelayCommand PrimaryButtonCommand
        {
            get
            {
                return _primaryButtonCommand
                    ?? (_primaryButtonCommand = new RelayCommand(
                    () =>
                    {
                        CurrentExpense.CreationMethod = String.Empty;
                        switch (SelectedSplitOption)
                        {
                            case ExpenseSplit.EQUALLY:
                                decimal eachPersonAmount = Math.Round(Convert.ToDecimal(CurrentExpense.Cost) / CurrentExpense.Users.Count(), 2);
                                decimal amountLeftOver = Convert.ToDecimal(CurrentExpense.Cost) - (eachPersonAmount * CurrentExpense.Users.Count());
                                foreach (var item in CurrentExpense.Users)
                                {
                                    item.OwedShare = eachPersonAmount.ToString();
                                }
                                //add the left over amount to the first person
                                if (amountLeftOver != 0)
                                {
                                    var enumerator = CurrentExpense.Users.GetEnumerator();
                                    enumerator.MoveNext();
                                    var user = enumerator.Current;
                                    decimal currentAmount = Convert.ToDecimal(user.OwedShare);
                                    decimal finalAmount = currentAmount + amountLeftOver;
                                    user.OwedShare = finalAmount.ToString();
                                }
                                CurrentExpense.CreationMethod = "equally";
                                break;
                            case ExpenseSplit.UNEQUALLY:
                                //Binding will handle this
                                break;
                            case ExpenseSplit.SHARES:
                                decimal totalShares = 0;
                                foreach (var item in CurrentExpense.Users)
                                {
                                    totalShares += Convert.ToDecimal(item.Share);
                                }
                                decimal expenseCost = Convert.ToDecimal(CurrentExpense.Cost);
                                decimal perShareCost = expenseCost / totalShares;
                                decimal totalSplit = 0;
                                foreach (var item in CurrentExpense.Users)
                                {
                                    item.OwedShare = Math.Round((perShareCost * Convert.ToDecimal(item.Share)), 2).ToString();
                                    totalSplit += Convert.ToDecimal(item.OwedShare);
                                }
                                decimal leftOver = expenseCost - totalSplit;
                                if(leftOver != 0)
                                {
                                    var enumerator = CurrentExpense.Users.GetEnumerator();
                                    enumerator.MoveNext();
                                    var user = enumerator.Current;
                                    decimal currentAmount = Convert.ToDecimal(user.OwedShare);
                                    decimal finalAmount = currentAmount + leftOver;
                                    user.OwedShare = finalAmount.ToString();
                                }
                                break;
                            default:
                                break;
                        }
                    }));
            }
        }
    }
}
