using Split_It.Model;

namespace Split_It.Converter.Repayment
{
    public class RepaymentBorrowLentConverter : BaseRepaymentConverter
    {
        protected override object getValue(Debt debt, User currentUser)
        {
            string value = "no balance";
            if (debt == null)
                return value;

            if (currentUser.id == debt.From)
            {
                value = "you borrowed";
            }
            else if (currentUser.id == debt.To)
            {
                value = "you lent";
            }

            return value;
        }
    }
}
