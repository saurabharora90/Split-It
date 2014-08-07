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
    class CurrentUserRequest : BaseRequest
    {
        public static String currentUserURL = "get_current_user";

        public CurrentUserRequest()
            : base()
        {
        }

        public void getCurrentUser(Action<User> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(currentUserURL);
            client.ExecuteAsync(request, reponse =>
                {
                    try
                    {
                        if (reponse.StatusCode != HttpStatusCode.OK && reponse.StatusCode != HttpStatusCode.NotModified)
                        {
                            CallbackOnFailure(reponse.StatusCode);
                            return;
                        }
                        Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(reponse.Content);
                        Newtonsoft.Json.Linq.JToken testToken = root["user"];
                        JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

                        User currentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(testToken.ToString(), settings);
                        CallbackOnSuccess(currentUser);
                    }

                    catch (Exception e)
                    {
                        CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
                    }
                });
        }
    }
}
