using Split_It_.Model;
using Split_It_.Request;
using Split_It_.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Controller
{
    public class ModifyDatabase
    {
        Action<bool, HttpStatusCode> callback;

        public ModifyDatabase(Action<bool, HttpStatusCode> callback)
        {
            this.callback = callback;
        }

        public void deleteExpense(int expenseId)
        {
            DeleteExpenseRequest request = new DeleteExpenseRequest(expenseId);
            request.deleteExpense(_ExpenseDeleted, _ExpenseDeletedFailed);
        }

        public void editExpense(Expense editedExpenseDetail)
        {

        }

        public void addExpense(Expense newExpense)
        {

        }

        private void _ExpenseDeleted(bool status)
        {
            syncDatabase();
        }

        private void _ExpenseDeletedFailed(HttpStatusCode statusCode)
        {
            callback(false, statusCode);
        }

        private void syncDatabase()
        {
            SyncDatabase databaseSync = new SyncDatabase(callback, false);
            databaseSync.performSync();
        }
    }
}
