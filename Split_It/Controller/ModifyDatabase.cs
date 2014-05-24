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
            request.deleteExpense(_OperationSucceded, _OperationFailed);
        }

        public void editExpense(Expense editedExpenseDetail)
        {

        }

        public void addExpense(Expense expense)
        {
            AddExpenseRequest request = new AddExpenseRequest(expense);
            request.addExpense(_OperationSucceded, _OperationFailed);
        }
        
        private void _OperationSucceded(bool status)
        {
            syncDatabase();
        }

        private void _OperationFailed(HttpStatusCode statusCode)
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
