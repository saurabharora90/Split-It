﻿using RestSharp;
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
            request.RootElement = "groups";
            try
            {
                client.ExecuteAsync<List<Group>>(request, reponse =>
                    {
                        List<Group> groups = reponse.Data;
                        CallbackOnSuccess(groups);
                    });
            }
            catch (Exception e)
            {
                CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
