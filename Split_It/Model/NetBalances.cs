using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Model
{
    class NetBalances : INotifyPropertyChanged
    {
        private string netBalance;
        private string positiveBalance;
        private string negativeBalance;

        public void setBalances(string currenyCode, double net, double positive, double negative)
        {
            currenyCode = currenyCode.ToUpper();
            netBalance = "balance (" + currenyCode + Convert.ToDouble(net) + ")";
            positiveBalance = "owes you (" + currenyCode + Convert.ToDouble(positive) + ")";
            negativeBalance = "you owe (" + currenyCode + Math.Abs(Convert.ToDouble(negative)) + ")";
        }

        public String NetBalance
        {
            get { return netBalance; }
            set
            {
                netBalance = value;
                OnPropertyChanged("NetBalance");
            }
        }

        public String PositiveBalance
        {
            get { return positiveBalance; }
            set
            {
                positiveBalance = value;
                OnPropertyChanged("PositiveBalance");
            }
        }

        public String NegativeBalance
        {
            get { return negativeBalance; }
            set
            {
                negativeBalance = value;
                OnPropertyChanged("NegativeBalance");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
