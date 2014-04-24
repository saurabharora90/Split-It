using Split_It_.Model;
using Split_It_.Request;
using Split_It_.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Controller
{
    class SetupFirstUse
    {
        Action<Uri> CallbackOnSuccess;
        SQLiteConnection dbConn;

        public SetupFirstUse(Action<Uri> callback)
        {
            this.CallbackOnSuccess = callback;
        }

        public void setup()
        {
            dbConn = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true);
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
            request.getCurrentUser(_CurrentUserDetailsReceived);   

            //Fetch expenses
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

            //Fire next request, i.e. get list of friends
            GetFriendsRequest request = new GetFriendsRequest();
            request.getAllFriends(Util.getLastUpdatedTime(), _FriendsDetailsRecevied);
        }

        private void _FriendsDetailsRecevied(List<User> friendsList)
        {
            dbConn.InsertAll(friendsList);

            //Now insert each friends picture and the balance of each user
            List<Picture> pictureList = new List<Picture>();
            List<Balance_User> userBalanceList = new List<Balance_User>();

            foreach (var friend in friendsList)
            {
                Picture picture = friend.picture;
                picture.user_id = friend.id;
                pictureList.Add(picture);

                foreach (var balance in friend.balance)
                {
                    balance.user_id = friend.id;
                    dbConn.Insert(balance);
                }
            }

            dbConn.InsertAll(pictureList);


            //Fetch groups
        }

        private void _GroupsDetailsReceived(List<Group> groupsList)
        {
            //Insert all groups

            //Insert group members

            //Insert debt_group
        }
    }
}
