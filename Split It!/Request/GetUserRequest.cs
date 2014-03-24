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
    class GetUserRequest : BaseRequest
    {
        public static String getUserURL = "get_user/{id}";

        public GetUserRequest()
            : base()
        {
        }

        public void getCurrentUser(int userId)
        {
            var request = new RestRequest(getUserURL);
            request.AddUrlSegment("id", userId.ToString());
            client.ExecuteAsync<RootObject>(request, reponse =>
                {
                    User currentUser = reponse.Data.user;
                });
        }
    }
}
