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


            //Fetch current user details
            CurrentUserRequest request = new CurrentUserRequest();
            request.getCurrentUser(_CurrentUserDetailsReceived);

            //Fetch list of friends

            //Fetch groups

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

        }
    }
}
