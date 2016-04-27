using Split_It.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Events
{
    public class BaseExpenseChangeEvent
    {
        public Expense ExpenseChanged { get; private set; }

        public BaseExpenseChangeEvent(Expense expense)
        {
            ExpenseChanged = expense;
        }
    }
}
