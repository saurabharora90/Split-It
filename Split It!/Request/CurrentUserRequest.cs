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
    class CurrentUserRequest
    {
        public static String currentUserURL = "get_current_user";

        public void getCurrentUser()
        {
            var client = new RestClient(Constants.SPLITWISE_API_URL);
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(Constants.consumerKey, Constants.consumerSecret, App.accessToken, App.accessTokenSecret);
            var request = new RestRequest(currentUserURL);
            client.ExecuteAsync<RootObject>(request, reponse =>
                {
                    User currentUser = reponse.Data.user;
                });
        }
    }
}
