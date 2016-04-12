using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Split_It.Model;

namespace Split_It.Converter.Repayment
{
    class RepaymentAmountConverter : BaseRepaymentConverter
    {
        protected override object getValue(Debt currentUserDebt, User currentUser)
        {
            if (currentUserDebt == null)
                return "0.00";
            else
                return currentUserDebt.CurrencyCode + currentUserDebt.Amount;
        }
    }
}
