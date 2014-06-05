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
            request.RootElement = "friend";

            request.AddParameter("user_email", email, ParameterType.GetOrPost);
            request.AddParameter("user_first_name", firstName, ParameterType.GetOrPost);
            
            if (!String.IsNullOrEmpty(lastName))
            {
                request.AddParameter("user_last_name", lastName, ParameterType.GetOrPost);
            }


            client.ExecuteAsync<List<User>>(request, reponse =>
                {
                    List<User> usersList = reponse.Data;
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
                });
        }
    }
}