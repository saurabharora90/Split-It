using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using Split_It_.Model;
using Split_It_.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public void getAllFriends(Action<List<User>> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(getFriendsURL);
            client.ExecuteAsync(request, reponse =>
                {
                    try
                    {
                        Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(reponse.Content);
                        Newtonsoft.Json.Linq.JToken testToken = root["friends"];
                        JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                        List<User> friends = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(testToken.ToString(), settings);
                        CallbackOnSuccess(friends);
                    }
                    catch (Exception e)
                    {
                        CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
                    }
                });
        }
    }
}
