using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Model
{
    public class UserBalance : GalaSoft.MvvmLight.ObservableObject
    {
        /// <summary>
        /// The <see cref="CurrencyCode" /> property's name.
        /// </summary>
        public const string CurrencyCodePropertyName = "CurrencyCode";

        private string _currencyCode = null;

        /// <summary>
        /// Sets and gets the CurrencyCode property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CurrencyCode
        {
            get
            {
                return _currencyCode;
            }

            set
            {
                if (_currencyCode == value)
                {
                    return;
                }

                _currencyCode = value;
                RaisePropertyChanged(CurrencyCodePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Amount" /> property's name.
        /// </summary>
        public const string AmountPropertyName = "Amount";

        private string _amount = null;

        /// <summary>
        /// Sets and gets the Amount property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Amount
        {
            get
            {
                return _amount;
            }

            set
            {
                if (_amount == value)
                {
                    return;
                }

                _amount = value;
                RaisePropertyChanged(AmountPropertyName);
            }
        }
    }
}
