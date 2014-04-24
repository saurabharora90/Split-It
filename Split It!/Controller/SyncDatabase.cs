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
    class SyncDatabase
    {
        bool firstSync;
        Action<Uri> CallbackOnSuccess;
        SQLiteConnection dbConn;

        public SyncDatabase(Action<Uri> callback, bool firstSync)
        {
            this.CallbackOnSuccess = callback;
            this.firstSync = firstSync;
        }

        public void performSync()
        {
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
            }

            //Fetch current user details
            CurrentUserRequest request = new CurrentUserRequest();
            request.getCurrentUser(_CurrentUserDetailsReceived);   

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
                }
                dbConn.InsertAll(friend.balance);
            }

            dbConn.InsertAll(pictureList);


            //Fetch groups
            GetGroupsRequest request = new GetGroupsRequest();
            request.getAllGroups(_GroupsDetailsReceived);
        }

        private void _GroupsDetailsReceived(List<Group> groupsList)
        {
            //Insert all groups
            dbConn.InsertAll(groupsList);

            //Insert group members
            //Insert debt_group
            foreach (var group in groupsList)
            {
                //only care about simplified debts as they are also returned if simplified debts are off

                //Also don't need the details (group_members and debt_group) for expenses which are not in any group, i.e group_id = 0;
                if (group.id == 0)
                    continue;
                else
                {
                    foreach (var debt in group.simplified_debts)
                    {
                        debt.group_id = group.id;
                    }
                    dbConn.InsertAll(group.simplified_debts);

                    foreach (var member in group.members)
                    {
                        Group_Members group_member = new Group_Members();
                        group_member.group_id = group.id;
                        group_member.user_id = member.id;
                        dbConn.Insert(group_member);
                    }
                }
            }

            //fetch expenses
        }
    }
}
