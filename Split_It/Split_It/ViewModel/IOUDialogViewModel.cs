﻿using GalaSoft.MvvmLight;
using Split_It.Model;
using Split_It.Model.Enum;
using Split_It.Service;
using Split_It.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.ViewModel
{
    public class IOUDialogViewModel : ViewModelBase
    {

        IContentDialogService _contentDialogService;
        public List<ExpenseSplit> SplitOptions { get; private set; }

        public IOUDialogViewModel(IContentDialogService contentDialogService)
        {
            _contentDialogService = contentDialogService;
            SplitOptions = new List<ExpenseSplit>(4);
            SplitOptions.Add(ExpenseSplit.EQUALLY);
            SplitOptions.Add(ExpenseSplit.YOU_OWE);
            SplitOptions.Add(ExpenseSplit.THEY_OWE);
            SplitOptions.Add(ExpenseSplit.UNEQUALLY);
        }

        #region Properties
        /// <summary>
        /// The <see cref="CurrentExpense" /> property's name.
        /// </summary>
        public const string CurrentExpensePropertyName = "CurrentExpense";

        private Expense _cuurentExpense = null;

        /// <summary>
        /// Sets and gets the CurrentExpense property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Expense CurrentExpense
        {
            get
            {
                return _cuurentExpense;
            }

            set
            {
                if (_cuurentExpense == value)
                {
                    return;
                }

                _cuurentExpense = value;
                RaisePropertyChanged(CurrentExpensePropertyName);
            }
        }

        public ExpenseSplit SelectedSplitOption
        {
            set
            {
                switch (value)
                {
                    case ExpenseSplit.EQUALLY:
                        decimal eachPersonAmount = System.Convert.ToDecimal(CurrentExpense.Cost) / 2;
                        foreach (var item in CurrentExpense.Users)
                        {
                            item.OwedShare = eachPersonAmount.ToString();
                        }
                        break;
                    case ExpenseSplit.UNEQUALLY:
                        _contentDialogService.showSplitDialog(CurrentExpense);
                        break;
                    case ExpenseSplit.THEY_OWE:
                        foreach (var item in CurrentExpense.Users)
                        {
                            if(item.UserId == AppState.CurrentUser.id)
                            {
                                item.PaidShare = CurrentExpense.Cost;
                                item.OwedShare = "0.0";
                            }
                            else
                            {
                                item.PaidShare = "0.0";
                                item.OwedShare = CurrentExpense.Cost;
                            }
                        }
                        break;
                    case ExpenseSplit.YOU_OWE:
                        foreach (var item in CurrentExpense.Users)
                        {
                            if (item.UserId != AppState.CurrentUser.id)
                            {
                                item.PaidShare = CurrentExpense.Cost;
                                item.OwedShare = "0.0";
                            }
                            else
                            {
                                item.PaidShare = "0.0";
                                item.OwedShare = CurrentExpense.Cost;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
    }
}
