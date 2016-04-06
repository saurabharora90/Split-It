using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Model
{
    public class Currency : GalaSoft.MvvmLight.ObservableObject
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
        /// 
        [JsonProperty(PropertyName = "currency_code")]
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
        /// The <see cref="Unit" /> property's name.
        /// </summary>
        public const string UnitPropertyName = "Unit";

        private string _unit = null;

        /// <summary>
        /// Sets and gets the Unit property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        /// 
        [JsonProperty(PropertyName = "unit")]
        public string Unit
        {
            get
            {
                return _unit;
            }

            set
            {
                if (_unit == value)
                {
                    return;
                }

                _unit = value;
                RaisePropertyChanged(UnitPropertyName);
            }
        }
    }
}
