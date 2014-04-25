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


        //Returns the list of friends along with the balance.
        public List<User> getFriends()
        {
            List<User> friendsList = dbConn.Query<User>("SELECT * FROM user ORDER BY first_name").ToList<User>();
            //remove the current user from the list as the user table also contains his details.
            foreach (var friend in friendsList)
            {
                if (friend.id == Util.getCurrentUserId())
                {
                    friendsList.Remove(friend);
                    break;
                }
            }

            return friendsList;
        }
    }
}
