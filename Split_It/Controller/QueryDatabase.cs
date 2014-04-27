using Split_It_.Model;
using Split_It_.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Controller
{
    class QueryDatabase
    {
        private static int EXPENSES_ROWS = 30;
        SQLiteConnection dbConn;

        public QueryDatabase()
        {
            dbConn = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true);
        }


        //Returns the list of friends along with the balance and their picture.
        public List<User> getAllFriends()
        {
            List<User> friendsList = dbConn.Query<User>("SELECT * FROM user ORDER BY first_name").ToList<User>();
            //remove the current user from the list as the user table also contains his details.
            for (var x = 0; x < friendsList.Count; x++)
            {
                if (friendsList[x].id == Util.getCurrentUserId())
                {
                    friendsList.Remove(friendsList[x]);
                    //As one element has been removed
                    x--;
                    continue;
                }

                object[] param = { friendsList[x].id };
                friendsList[x].balance = dbConn.Query<Balance_User>("SELECT * FROM balance_user WHERE user_id= ?", param).ToList<Balance_User>();
                friendsList[x].picture = getUserPicture(friendsList[x].id);
            }

            return friendsList;
        }

        public List<Expense> getAllExpenses(int pageNo=0)
        {
            int startLimit = EXPENSES_ROWS * pageNo;
            int endLimit = EXPENSES_ROWS * (pageNo + 1);
            object[] param = {startLimit, endLimit};
            List<Expense> expensesList = dbConn.Query<Expense>("SELECT * FROM expense ORDER BY datetime(created_at) DESC LIMIT ?,?", param).ToList<Expense>();
            
            //Get list of repayments for expense.
            //Get the created by, updated by and deleted by user
            //Get the expense share per user. Within each expense user, fill in the user details.
            for (var x = 0; x < expensesList.Count; x++)
            {
                object[] expenseParam = { expensesList[x].id };
                expensesList[x].repayments = dbConn.Query<Debt_Expense>("SELECT * FROM debt_expense WHERE expense_id= ?", expenseParam).ToList<Debt_Expense>();
                expensesList[x].created_by = getUserDetails(expensesList[x].created_by_user_id);
                
                if(expensesList[x].updated_by_user_id!=0)
                    expensesList[x].updated_by = getUserDetails(expensesList[x].updated_by_user_id);

                if (expensesList[x].deleted_by_user_id != 0)
                    expensesList[x].deleted_by = getUserDetails(expensesList[x].deleted_by_user_id);
                
                expensesList[x].users = dbConn.Query<Expense_Share>("SELECT * FROM expense_share WHERE expense_id= ?", expenseParam).ToList<Expense_Share>();

                for (var y = 0; y < expensesList[x].users.Count; y++)
                {
                    expensesList[x].users[y].user = getUserDetails(expensesList[x].users[y].user_id);
                }
            }
            
            return expensesList;
        }

        public void closeDatabaseConnection()
        {
            dbConn.Close();
        }

        private User getUserDetails(int userId)
        {
            User user = dbConn.Query<User>("SELECT * FROM user WHERE id= ?", new object[] { userId }).First();
            user.picture = getUserPicture(userId);
            return user;
        }

        private Picture getUserPicture(int userId)
        {
            return dbConn.Query<Picture>("SELECT * FROM picture WHERE user_id= ?", new object[] { userId }).First();
        }
    }
}
