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
                friendsList[x].picture = dbConn.Query<Picture>("SELECT * FROM picture WHERE user_id= ?", param).First(); ;
            }

            return friendsList;
        }

        public List<Expense> getAllExpenses()
        {
            
        }
    }
}
