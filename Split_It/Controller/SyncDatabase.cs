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
    class SyncDatabase
    {
        bool firstSync;
        Action<bool, HttpStatusCode> CallbackOnSuccess;
        SQLiteConnection dbConn;

        public SyncDatabase(Action<bool, HttpStatusCode> callback, bool firstSync)
        {
            this.CallbackOnSuccess = callback;
            this.firstSync = firstSync;
        }

        public void performSync()
        {
            if (!Util.checkNetworkConnection())
            {
                CallbackOnSuccess(false, HttpStatusCode.ServiceUnavailable);
                return;
            }
            dbConn = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true);
            if (firstSync)
            {
                //Delete all the data in the database as first use will also be called after logout.
                dbConn.DeleteAll<User>();
                dbConn.DeleteAll<Expense>();
                dbConn.DeleteAll<Group>();
                dbConn.DeleteAll<Picture>();
                dbConn.DeleteAll<Balance_User>();
                dbConn.DeleteAll<Debt_Expense>();
                dbConn.DeleteAll<Debt_Group>();
                dbConn.DeleteAll<Expense_Share>();
                dbConn.DeleteAll<Group_Members>();

                //Fetch current user details
                CurrentUserRequest request = new CurrentUserRequest();
                request.getCurrentUser(_CurrentUserDetailsReceived, _OnErrorReceived);
            }
            else
            {
                //fetch expenses
                GetExpensesRequest request = new GetExpensesRequest();
                request.getAllExpenses(_ExpensesDetailsReceived, _OnErrorReceived);
            }

        }

        private void _CurrentUserDetailsReceived(User currentUser)
        {
            //Insert user details to database
            dbConn.Insert(currentUser);

            //Insert picture into database
            currentUser.picture.user_id = currentUser.id;
            dbConn.Insert(currentUser.picture);
            
            //Save current user id in isolated storage
            Util.setCurrentUserId(currentUser.id);


            //fetch expenses
            GetExpensesRequest request = new GetExpensesRequest();
            request.getAllExpenses(_ExpensesDetailsReceived, _OnErrorReceived);
        }

        private void _ExpensesDetailsReceived(List<Expense> expensesList)
        {
            //if no expenses were recieved, then there wasnt any change in user balances or groups.
            if (expensesList == null || expensesList.Count == 0)
            {
                dbConn.Close();
                CallbackOnSuccess(true, HttpStatusCode.OK);
                return;
            }


            dbConn.BeginTransaction();
            //Insert expenses
            foreach (var expense in expensesList)
            {
                //The api returns the entire user details of the created by, updated by and deleted by users.
                //But we only need to store their id's into the database
                if (expense.created_by != null)
                    expense.created_by_user_id = expense.created_by.id;

                if (expense.updated_by != null)
                    expense.updated_by_user_id = expense.updated_by.id;

                if (expense.deleted_by != null)
                    expense.deleted_by_user_id = expense.deleted_by.id;

                dbConn.InsertOrReplace(expense);
            }

            //Insert debt of each expense (repayments)
            //Insert expense share users
            foreach (var expense in expensesList)
            {
                //delete users and repayments for this specific expense id as they might have been edited since the last update
                object[] param = { expense.id };
                dbConn.Query<Debt_Expense>("Delete FROM debt_expense WHERE expense_id= ?", param);
                dbConn.Query<Expense_Share>("Delete FROM expense_share WHERE expense_id= ?", param);

                foreach (var repayment in expense.repayments)
                {
                    repayment.expense_id = expense.id;
                    dbConn.InsertOrReplace(repayment);
                }

                foreach (var expenseUser in expense.users)
                {
                    expenseUser.expense_id = expense.id;
                    expenseUser.user_id = expenseUser.user.id;
                    dbConn.InsertOrReplace(expenseUser);
                }

                //dbConn.InsertAll(expense.repayments);
                //dbConn.InsertAll(expense.users);
            }

            dbConn.Commit();

            Util.setLastUpdatedTime();

            //Fire next request, i.e. get list of friends
            GetFriendsRequest request = new GetFriendsRequest();
            request.getAllFriends(_FriendsDetailsRecevied);
        }

        private void _FriendsDetailsRecevied(List<User> friendsList)
        {
            //dbConn.InsertAll(friendsList);

            //Now insert each friends picture and the balance of each user
            List<Picture> pictureList = new List<Picture>();
            List<Balance_User> userBalanceList = new List<Balance_User>();
            
            dbConn.BeginTransaction();
            foreach (var friend in friendsList)
            {
                dbConn.InsertOrReplace(friend);

                Picture picture = friend.picture;
                picture.user_id = friend.id;
                pictureList.Add(picture);
                dbConn.InsertOrReplace(picture);

                //delete all the balances of the friends as they might have changed since the last update
                //if (friend.balance == null || friend.balance.Count == 0)
                //{
                    object[] param = { friend.id };
                    dbConn.Query<Balance_User>("Delete FROM balance_user WHERE user_id= ?", param);
                //}

                foreach (var balance in friend.balance)
                {
                    balance.user_id = friend.id;
                    dbConn.InsertOrReplace(balance);
                }
                //dbConn.InsertAll(friend.balance);
            }

            //dbConn.InsertAll(pictureList);
            dbConn.Commit();

            //Fetch groups
            GetGroupsRequest request = new GetGroupsRequest();
            request.getAllGroups(_GroupsDetailsReceived);
        }

        private void _GroupsDetailsReceived(List<Group> groupsList)
        {
            //Insert all groups
            //dbConn.InsertAll(groupsList);

            dbConn.BeginTransaction();
            //Insert group members
            //Insert debt_group
            foreach (var group in groupsList)
            {
                dbConn.InsertOrReplace(group);
                //only care about simplified debts as they are also returned if simplified debts are off

                //Also don't need the details (group_members and debt_group) for expenses which are not in any group, i.e group_id = 0;
                if (group.id == 0)
                    continue;
                else
                {
                    //delete simplified debts and group member as they might have changed since the last update
                    object[] param = { group.id };
                    dbConn.Query<Group_Members>("Delete FROM group_members WHERE group_id= ?", param);
                    dbConn.Query<Debt_Group>("Delete FROM debt_group WHERE group_id= ?", param);

                    foreach (var debt in group.simplified_debts)
                    {
                        debt.group_id = group.id;
                        dbConn.InsertOrReplace(debt);
                    }
                    //dbConn.InsertAll(group.simplified_debts);

                    foreach (var member in group.members)
                    {
                        Group_Members group_member = new Group_Members();
                        group_member.group_id = group.id;
                        group_member.user_id = member.id;
                        dbConn.InsertOrReplace(group_member);
                    }
                }
            }

            dbConn.Commit();

            GetCurrenciesRequest request = new GetCurrenciesRequest();
            request.getSupportedCurrencies(_CurrenciesReceived);
        }

        private void _CurrenciesReceived(List<Currency> currencyList)
        {
            dbConn.DeleteAll<Currency>();
            dbConn.InsertAll(currencyList);
            dbConn.Close();
            CallbackOnSuccess(true, HttpStatusCode.OK);
        }

        private void _OnErrorReceived(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized:
                    CallbackOnSuccess(false, HttpStatusCode.Unauthorized);
                    break;
                default:
                    CallbackOnSuccess(false,statusCode);
                    break;
            }
        }
    }
}
