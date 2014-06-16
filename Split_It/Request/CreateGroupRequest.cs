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
            request.RootElement = "group";

            request.AddParameter("name", groupToAdd.name, ParameterType.GetOrPost);

            int count = 0;
            foreach (var user in groupToAdd.members)
            {
                string idKey = String.Format("users__{0}__user_id", count);
                request.AddParameter(idKey, user.id, ParameterType.GetOrPost);

                count++;
            }


            client.ExecuteAsync<List<Group>>(request, reponse =>
                {
                    List<Group> groupsList = reponse.Data;
                    if (groupsList != null)
                    {
                        Group group = groupsList[0];
                        if (group.id != 0)
                            CallbackOnSuccess(group);
                        else
                            CallbackOnFailure(reponse.StatusCode);
                    }
                    else
                        CallbackOnFailure(reponse.StatusCode);
                });
        }
    }
}
