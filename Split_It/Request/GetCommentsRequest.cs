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
    class GetCommentsRequest : BaseRequest
    {
        public static String getFriendsURL = "get_comments";
        private int expenseId;

        public GetCommentsRequest(int expenseId)
            : base()
        {
            this.expenseId = expenseId;
        }

        public void getAllFriends(Action<List<User>> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(getFriendsURL);
            request.AddParameter("expense_id", expenseId, ParameterType.GetOrPost);
            //request.RootElement = "friends";
            client.ExecuteAsync<List<User>>(request, reponse =>
                {
                    if(reponse.StatusCode != HttpStatusCode.OK && reponse.StatusCode != HttpStatusCode.NotModified)
                    {
                        CallbackOnFailure(reponse.StatusCode);
                        return;
                    }
                    List<User> friends = reponse.Data;
                    CallbackOnSuccess(friends);
                });
        }
    }
}
