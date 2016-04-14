using Split_It.Model;
using System.Diagnostics;

namespace Split_It.Converter.Repayment
{
    class RepaymentAmountConverter : BaseRepaymentConverter
    {
        protected override object getValue(Debt currentUserDebt, User currentUser)
        {
            if (currentUserDebt == null)
                return "0.00";
            else
                return currentUserDebt.Amount;
        }
    }
}
