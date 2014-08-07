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
    class CreateFriendRequest : BaseRequest
    {
        public static String createFriendURL = "create_friend";
        string email, firstName, lastName;

        public CreateFriendRequest(string email, string firstName, string lastName)
            : base()
        {
            this.email = email;
            this.firstName = firstName;
            this.lastName = lastName;
        }

        public void createFriend(Action<User> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(createFriendURL, Method.POST);

            request.AddParameter("user_email", email, ParameterType.GetOrPost);
            request.AddParameter("user_first_name", firstName, ParameterType.GetOrPost);
            
            if (!String.IsNullOrEmpty(lastName))
            {
                request.AddParameter("user_last_name", lastName, ParameterType.GetOrPost);
            }

            client.ExecuteAsync(request, reponse =>
                {
                    try
                    {
                        Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(reponse.Content);
                        Newtonsoft.Json.Linq.JToken testToken = root["friend"];
                        JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                        List<User> usersList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(testToken.ToString(), settings);
                        if (usersList != null)
                        {
                            User user = usersList[0];
                            if (user.id != 0)
                                CallbackOnSuccess(user);
                            else
                                CallbackOnFailure(reponse.StatusCode);
                        }
                        else
                            CallbackOnFailure(reponse.StatusCode);
                    }
                    catch (Exception e)
                    {
                        CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
                    }
                });
        }
    }
}
