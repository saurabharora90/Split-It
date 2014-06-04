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
            UpdateExpenseRequest request = new UpdateExpenseRequest(editedExpenseDetail);
            request.updateExpense(_OperationSucceded, _OperationFailed);
        }

        public void addExpense(Expense expense)
        {
            AddExpenseRequest request = new AddExpenseRequest(expense);
            request.addExpense(_OperationSucceded, _OperationFailed);
        }

        public void createFriend(string email, string firstName, string lastName)
        {
            CreateFriendRequest request = new CreateFriendRequest(email, firstName, lastName);
            request.createFriend(_FriendAdded, _OperationFailed);
        }

        private void _FriendAdded(User friend)
        {
            //add user to database and to friends list in App.xaml
            App.friendsList.Add(friend);
            
            SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true);
            dbConn.Insert(friend);
            friend.picture.user_id = friend.id;
            dbConn.Insert(friend.picture);
            dbConn.Close();

            callback(true, HttpStatusCode.OK);
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
