using Split_It.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Events
{
    public class ExpenseEditedEvent : BaseExpenseChangeEvent
    {
        public ExpenseEditedEvent(Expense expense) : base(expense)
        {

        }
    }
}
