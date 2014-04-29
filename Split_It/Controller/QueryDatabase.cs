﻿using Split_It_.Model;
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
        private static int EXPENSES_ROWS = 10;
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
                    App.currentUser = friendsList[x];
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

            //Only retrieve expenses that have not been deleted
            List<Expense> expensesList = dbConn.Query<Expense>("SELECT * FROM expense WHERE deleted_by=0 ORDER BY datetime(created_at) DESC LIMIT ?,?", param).ToList<Expense>();
            
            //Get list of repayments for expense.
            //Get the created by, updated by and deleted by user
            //Get the expense share per user. Within each expense user, fill in the user details.
            for (var x = 0; x < expensesList.Count; x++)
            {
                expensesList[x].repayments = getExpenseRepayments(expensesList[x].id);
                expensesList[x].created_by = getUserDetails(expensesList[x].created_by_user_id);
                
                if(expensesList[x].updated_by_user_id!=0)
                    expensesList[x].updated_by = getUserDetails(expensesList[x].updated_by_user_id);

                if (expensesList[x].deleted_by_user_id != 0)
                    expensesList[x].deleted_by = getUserDetails(expensesList[x].deleted_by_user_id);

                expensesList[x].users = getExpenseShareUsers(expensesList[x].id);

                for (var y = 0; y < expensesList[x].users.Count; y++)
                {
                    expensesList[x].users[y].user = getUserDetails(expensesList[x].users[y].user_id);
                }
            }
            
            return expensesList;
        }

        public List<Expense> getExpensesForUser(int userId, int pageNo=0)
        {
            int startLimit = EXPENSES_ROWS * pageNo;
            int endLimit = EXPENSES_ROWS * (pageNo + 1);

            //the expenses for for a user is a combination of expenses paid by him and owe by me
            //or
            //paid by me and owed by him
            //Only retrieve expenses that have not been deleted

            object[] param = { Util.getCurrentUserId(), userId, userId, Util.getCurrentUserId(), startLimit, endLimit };
            List<Expense> expensesList = dbConn.Query<Expense>("SELECT expense.id, expense.group_id, expense.description, expense.details, expense.payment, expense.transaction_confirmed, expense.creation_method, expense.cost, expense.currency_code, expense.date, expense.created_by, expense.created_at, expense.updated_by, expense.updated_at, expense.deleted_at, expense.deleted_by FROM expense INNER JOIN debt_expense ON expense.id = debt_expense.expense_id WHERE (debt_expense.\"from\" = ? AND debt_expense.\"to\" = ?) OR (debt_expense.\"from\" = ? AND debt_expense.\"to\" = ?) ORDER BY datetime(created_at) DESC LIMIT ?,?", param).ToList<Expense>();

            if (expensesList == null && expensesList.Count == 0)
                return null;

            //Get list of repayments for expense.
            //Get the created by, updated by and deleted by user
            //Get the expense share per user. Within each expense user, fill in the user details.
            for (var x = 0; x < expensesList.Count; x++)
            {
                //display amount details specific to user
                expensesList[x].displayType = Expense.DISPLAY_FOR_SPECIFIC_USER;
                expensesList[x].specificUserId = userId;

                expensesList[x].repayments = getExpenseRepayments(expensesList[x].id);
                expensesList[x].created_by = getUserDetails(expensesList[x].created_by_user_id);

                if (expensesList[x].updated_by_user_id != 0)
                    expensesList[x].updated_by = getUserDetails(expensesList[x].updated_by_user_id);

                if (expensesList[x].deleted_by_user_id != 0)
                    expensesList[x].deleted_by = getUserDetails(expensesList[x].deleted_by_user_id);

                expensesList[x].users = getExpenseShareUsers(expensesList[x].id);

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

        private List<Expense_Share> getExpenseShareUsers(int expenseId)
        {
            return dbConn.Query<Expense_Share>("SELECT * FROM expense_share WHERE expense_id= ?", new object[] { expenseId }).ToList<Expense_Share>();
        }

        private List<Debt_Expense> getExpenseRepayments(int expenseId)
        {
            return dbConn.Query<Debt_Expense>("SELECT * FROM debt_expense WHERE expense_id= ?", new object[] { expenseId }).ToList<Debt_Expense>();
        }

        private User getUserDetails(int userId)
        {
            List<User> users = dbConn.Query<User>("SELECT * FROM user WHERE id= ?", new object[] { userId });
            if (users != null && users.Count != 0)
            {
                User user = users.First();
                user.picture = getUserPicture(userId);
                return user;
            }
            else
                return null;
        }

        private Picture getUserPicture(int userId)
        {
            return dbConn.Query<Picture>("SELECT * FROM picture WHERE user_id= ?", new object[] { userId }).First();
        }
    }
}