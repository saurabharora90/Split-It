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
    class CreateGroupRequest : BaseRequest
    {
        public static String createGroupURL = "create_group";
        Group groupToAdd;

        public CreateGroupRequest(Group group)
            : base()
        {
            this.groupToAdd = group;
        }

        public void createGroup(Action<Group> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(createGroupURL, Method.POST);

            request.AddParameter("name", groupToAdd.name, ParameterType.GetOrPost);

            int count = 0;
            foreach (var user in groupToAdd.members)
            {
                string idKey = String.Format("users__{0}__user_id", count);
                request.AddParameter(idKey, user.id, ParameterType.GetOrPost);

                count++;
            }


            client.ExecuteAsync(request, reponse =>
                {
                    try
                    {
                        Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(reponse.Content);
                        Newtonsoft.Json.Linq.JToken testToken = root["group"];
                        JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                        Group group = Newtonsoft.Json.JsonConvert.DeserializeObject<Group>(testToken.ToString(), settings);
                        if (group != null)
                        {
                            if (group.id != 0)
                                CallbackOnSuccess(group);
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
