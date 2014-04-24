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
    class GetFriendsRequest : BaseRequest
    {
        public static String getFriendsURL = "get_friends";

        public GetFriendsRequest()
            : base()
        {
        }

        public void getAllFriends(int lastUpdatedTime, Action<List<User>> CallbackOnSuccess)
        {
            var request = new RestRequest(getFriendsURL);
            request.RootElement = "friends";
            client.ExecuteAsync<List<User>>(request, reponse =>
                {
                    List<User> friends = reponse.Data;
                    CallbackOnSuccess(friends);
                });
        }
    }
}
