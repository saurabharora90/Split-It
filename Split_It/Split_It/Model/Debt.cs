using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Model
{
    public class Debt : ObservableObject
    {
        /// <summary>
        /// The <see cref="From" /> property's name.
        /// </summary>
        public const string FromPropertyName = "From";

        private int _from;

        /// <summary>
        /// Sets and gets the From property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int From
        {
            get
            {
                return _from;
            }

            set
            {
                if (_from == value)
                {
                    return;
                }

                _from = value;
                RaisePropertyChanged(FromPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="To" /> property's name.
        /// </summary>
        public const string ToPropertyName = "To";

        private int _to;

        /// <summary>
        /// Sets and gets the To property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int To
        {
            get
            {
                return _to;
            }

            set
            {
                if (_to == value)
                {
                    return;
                }

                _to = value;
                RaisePropertyChanged(ToPropertyName);
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
    }
}
