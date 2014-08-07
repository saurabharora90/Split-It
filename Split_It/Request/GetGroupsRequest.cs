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
    class GetGroupsRequest : BaseRequest
    {
        public static String getGroupsURL = "get_groups";

        public GetGroupsRequest()
            : base()
        {
        }

        public void getAllGroups(Action<List<Group>> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(getGroupsURL);
            client.ExecuteAsync(request, reponse =>
                {
                    try
                    {
                        Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(reponse.Content);
                        Newtonsoft.Json.Linq.JToken testToken = root["groups"];
                        JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                        List<Group> groups = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Group>>(testToken.ToString(), settings);
                        CallbackOnSuccess(groups);
                    }
                    catch (Exception e)
                    {
                        CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
                    }
                });
        }
    }
}
