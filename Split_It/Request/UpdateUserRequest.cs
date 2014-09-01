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
    class UpdateUserRequest : BaseRequest
    {
        public static String updateUserURL = "update_user/{id}";
        User updatedUser;

        public UpdateUserRequest(User user)
            : base()
        {
            this.updatedUser = user;
        }

        public void updateUser(Action<User, HttpStatusCode> CallbackOnSuccess)
        {
            var request = new RestRequest(updateUserURL, Method.POST);
            request.AddUrlSegment("id", updatedUser.id.ToString());

            request.AddParameter("first_name", updatedUser.first_name, ParameterType.GetOrPost);
            request.AddParameter("last_name", updatedUser.last_name, ParameterType.GetOrPost);
            request.AddParameter("email", updatedUser.email, ParameterType.GetOrPost);

            if (!String.IsNullOrEmpty(updatedUser.default_currency))
                request.AddParameter("default_currency", updatedUser.default_currency, ParameterType.GetOrPost);

            client.ExecuteAsync(request, reponse =>
            {
                try
                {
                    if (reponse.StatusCode != HttpStatusCode.OK && reponse.StatusCode != HttpStatusCode.NotModified)
                    {
                        CallbackOnSuccess(null, reponse.StatusCode);
                        return;
                    }
                    Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(reponse.Content);
                    Newtonsoft.Json.Linq.JToken testToken = root["user"];
                    JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

                    User currentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(testToken.ToString(), settings);
                    CallbackOnSuccess(currentUser, HttpStatusCode.OK);
                }

                catch (Exception e)
                {
                    CallbackOnSuccess(null, HttpStatusCode.ServiceUnavailable);
                }
            });
        }
    }
}
