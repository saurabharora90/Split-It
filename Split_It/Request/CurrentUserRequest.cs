using RestSharp;
using RestSharp.Authenticators;
using Split_It_.Model;
using Split_It_.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Request
{
    class CurrentUserRequest : BaseRequest
    {
        public static String currentUserURL = "get_current_user";

        public CurrentUserRequest()
            : base()
        {
        }

        public void getCurrentUser(Action<User> CallbackOnSuccess)
        {
            var request = new RestRequest(currentUserURL);
            request.RootElement = "user";
            client.ExecuteAsync<User>(request, reponse =>
                {
                    User currentUser = reponse.Data;
                    CallbackOnSuccess(currentUser);
                });
        }
    }
}
