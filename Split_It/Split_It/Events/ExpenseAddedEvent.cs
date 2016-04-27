using Split_It.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Events
{
    public class ExpenseAddedEvent : BaseExpenseChangeEvent
    {
        public ExpenseAddedEvent(Expense expense) : base(expense)
        {

        }
    }
}
