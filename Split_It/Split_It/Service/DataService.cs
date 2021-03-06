﻿using RestSharp.Portable;
using RestSharp.Portable.Authenticators;
using Split_It.Model;
using Split_It.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp.Portable.WebRequest;
using GalaSoft.MvvmLight.Messaging;
using Split_It.Events;

namespace Split_It.Service
{
    public class DataService : IDataService
    {
        private RestClient _splitwiseClient;
        private JsonSerializerSettings _jsonSettings;

        public DataService()
        {
            _splitwiseClient = new RestClient(Constants.SPLITWISE_API_URL) { IgnoreResponseStatusCode = true };
            _splitwiseClient.Authenticator = OAuth1Authenticator.ForProtectedResource(Constants.consumerKey, Constants.consumerSecret, AppState.AccessToken, AppState.AccessTokenSecret);

            _jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new DeliminatorSeparatedPropertyNamesContractResolver('_') };
        }

        private RestClient getRestClient()
        {
            RestClient client = new RestClient(Constants.SPLITWISE_API_URL) { IgnoreResponseStatusCode = true };
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(Constants.consumerKey, Constants.consumerSecret, AppState.AccessToken, AppState.AccessTokenSecret);

            return client;
        }

        private string getStringFromResponse(IRestResponse response)
        {
            return System.Text.Encoding.UTF8.GetString(response.RawBytes, 0, response.RawBytes.Length);
        }

        #region ParallelRequests
        // The following requests are supposed to run in parallel and hence they use a different rest client for each request
        //https://github.com/FubarDevelopment/restsharp.portable/issues/19

        public async Task<User> getCurrentUser()
        {
            var request = new RestRequest("get_current_user");
            var response = await _splitwiseClient.Execute(request);
            if (response.IsSuccess)
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
                Newtonsoft.Json.Linq.JToken testToken = root["user"];

                return JsonConvert.DeserializeObject<User>(testToken.ToString(), _jsonSettings);
            }
            else
            {
                Messenger.Default.Send(new ApiErrorEvent(response.StatusCode));
                User user = new User();
                user.FirstName = "";
                user.id = -1;
                return user;
            }
        }

        public async Task<IEnumerable<Friend>> getFriendsList()
        {
            RestClient client = getRestClient();
            var request = new RestRequest("get_friends");
            var response = await client.Execute(request);
            if (response.IsSuccess)
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
                Newtonsoft.Json.Linq.JToken testToken = root["friends"];
                client.Dispose();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Friend>>(testToken.ToString(), _jsonSettings);
            }
            else
            {
                return new List<Friend>();
            }
        }

        public async Task<IEnumerable<Group>> getGroupsList()
        {
            RestClient client = getRestClient();
            var request = new RestRequest("get_groups");
            var response = await client.Execute(request);
            if (response.IsSuccess)
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
                Newtonsoft.Json.Linq.JToken testToken = root["groups"];

                return JsonConvert.DeserializeObject<IEnumerable<Group>>(testToken.ToString(), _jsonSettings);
            }
            else
            {
                return new List<Group>();
            }
        }

        public async Task<IEnumerable<Friendship>> getFriendShip()
        {
            RestClient client = getRestClient();
            var request = new RestRequest("get_friendships");
            var response = await client.Execute(request);
            if (response.IsSuccess)
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
                Newtonsoft.Json.Linq.JToken testToken = root["friendships"];
                client.Dispose();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Friendship>>(testToken.ToString(), _jsonSettings);
            }
            else
            {
                return new List<Friendship>();
            }
        }

        public async Task<IEnumerable<Notification>> getNotifications()
        {
            RestClient client = getRestClient();
            var request = new RestRequest("get_notifications");
            request.AddParameter("limit", 20, ParameterType.GetOrPost);
            var response = await client.Execute(request);
            if (response.IsSuccess)
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
                Newtonsoft.Json.Linq.JToken testToken = root["notifications"];
                client.Dispose();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Notification>>(testToken.ToString(), _jsonSettings);
            }
            else
            {
                return new List<Notification>();
            }
        }

        #endregion

        public async Task<IEnumerable<Expense>> getExpenseForFriend(int friendshipId, int limit, int offset = 0)
        {
            var request = new RestRequest("get_expenses");
            request.AddParameter("offset", offset, ParameterType.GetOrPost);
            request.AddParameter("friendship_id", friendshipId, ParameterType.GetOrPost);
            request.AddParameter("limit", limit, ParameterType.GetOrPost);
            var response = await _splitwiseClient.Execute(request);
            if (response.IsSuccess)
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
                Newtonsoft.Json.Linq.JToken testToken = root["expenses"];

                return JsonConvert.DeserializeObject<IEnumerable<Expense>>(testToken.ToString(), _jsonSettings);
            }
            else
            {
                Messenger.Default.Send(new ApiErrorEvent(response.StatusCode));
                return new List<Expense>();
            }
        }

        public async Task<IEnumerable<Expense>> getExpenseForGroup(int groupId, int limit, int offset = 0)
        {
            var request = new RestRequest("get_expenses");
            request.AddParameter("offset", offset, ParameterType.GetOrPost);
            request.AddParameter("group_id", groupId, ParameterType.GetOrPost);
            request.AddParameter("limit", limit, ParameterType.GetOrPost);
            var response = await _splitwiseClient.Execute(request);
            if (response.IsSuccess)
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
                Newtonsoft.Json.Linq.JToken testToken = root["expenses"];

                return JsonConvert.DeserializeObject<IEnumerable<Expense>>(testToken.ToString(), _jsonSettings);
            }
            else
            {
                Messenger.Default.Send(new ApiErrorEvent(response.StatusCode));
                return new List<Expense>();
            }
        }

        public async Task<bool> deleteExpense(int expenseId)
        {
            var request = new RestRequest("delete_expense/{id}", Method.POST);
            request.AddUrlSegment("id", expenseId.ToString());
            var response = await _splitwiseClient.Execute(request);
            if (response.IsSuccess)
            {
                var delete = Newtonsoft.Json.JsonConvert.DeserializeObject<DeleteExpense>(getStringFromResponse(response), _jsonSettings);
                return delete.success;
            }
            else
            {
                return false;
            }
        }

        public async Task<Friend> getFriendInfo(int friendId)
        {
            var request = new RestRequest("get_friend/{id}", Method.POST);
            request.AddUrlSegment("id", friendId.ToString());
            var response = await _splitwiseClient.Execute(request);

            Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
            Newtonsoft.Json.Linq.JToken testToken = root["friend"];
            return JsonConvert.DeserializeObject<Friend>(testToken.ToString(), _jsonSettings);
        }

        public async Task<Group> getGroupInfo(int groupId)
        {
            var request = new RestRequest("get_group/{id}", Method.POST);
            request.AddUrlSegment("id", groupId.ToString());
            var response = await _splitwiseClient.Execute(request);

            Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
            Newtonsoft.Json.Linq.JToken testToken = root["group"];
            return JsonConvert.DeserializeObject<Group>(testToken.ToString(), _jsonSettings);
        }

        public async Task<IEnumerable<Comment>> getComments(int expenseId)
        {
            var request = new RestRequest("get_comments", Method.POST);
            request.AddParameter("expense_id", expenseId, ParameterType.GetOrPost);
            var response = await _splitwiseClient.Execute(request);
            if (response.IsSuccess)
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
                Newtonsoft.Json.Linq.JToken testToken = root["comments"];
                return JsonConvert.DeserializeObject<IEnumerable<Comment>>(testToken.ToString(), _jsonSettings);
            }
            else
            {
                return new List<Comment>();
            }
        }

        public async Task<Comment> postCommentOnExpense(int expenseId, string comment)
        {
            var request = new RestRequest("create_comment");
            request.AddParameter("expense_id", expenseId, ParameterType.GetOrPost);
            request.AddParameter("content", comment, ParameterType.GetOrPost);
            var response = await _splitwiseClient.Execute(request);
            if (response.IsSuccess)
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
                Newtonsoft.Json.Linq.JToken testToken = root["comment"];
                return JsonConvert.DeserializeObject<Comment>(testToken.ToString(), _jsonSettings);
            }
            else
            {
                Messenger.Default.Send(new ApiErrorEvent(response.StatusCode));
                return null;
            }
        }

        public async Task<IEnumerable<Expense>> createExpense(Expense paymentExpense)
        {
            RestRequest request = null;
            if(paymentExpense.Id == 0)
                request = new RestRequest("create_expense", Method.POST);
            else
            {
                request = new RestRequest("update_expense/{id}", Method.POST);
                request.AddUrlSegment("id", paymentExpense.Id.ToString());
            }

            if (paymentExpense.Payment)
                request.AddParameter("payment", "true", ParameterType.GetOrPost);
            else
                request.AddParameter("payment", "false", ParameterType.GetOrPost);

            request.AddParameter("cost", Convert.ToString(Convert.ToDouble(paymentExpense.Cost), System.Globalization.CultureInfo.InvariantCulture), ParameterType.GetOrPost);
            request.AddParameter("description", paymentExpense.Description, ParameterType.GetOrPost);

            if (!String.IsNullOrEmpty(paymentExpense.CurrencyCode))
                request.AddParameter("currency_code", paymentExpense.CurrencyCode, ParameterType.GetOrPost);

            if (!String.IsNullOrEmpty(paymentExpense.CreationMethod))
            {
                request.AddParameter("creation_method", paymentExpense.CreationMethod, ParameterType.GetOrPost);
            }

            if (!String.IsNullOrEmpty(paymentExpense.Details))
            {
                request.AddParameter("details", paymentExpense.Details, ParameterType.GetOrPost);
            }

            if (!String.IsNullOrEmpty(paymentExpense.Date)) //Repesents the date this expense belongs to
            {
                request.AddParameter("date", paymentExpense.Date, ParameterType.GetOrPost);
            }

            if (paymentExpense.GroupId != 0)
            {
                request.AddParameter("group_id", paymentExpense.GroupId, ParameterType.GetOrPost);
            }

            int count = 0;
            foreach (var user in paymentExpense.Users)
            {
                string idKey = String.Format("users__array_{0}__user_id", count);
                string paidKey = String.Format("users__array_{0}__paid_share", count);
                string owedKey = String.Format("users__array_{0}__owed_share", count);
                request.AddParameter(idKey, user.UserId, ParameterType.GetOrPost);
                request.AddParameter(paidKey, Convert.ToString(Convert.ToDouble(user.PaidShare), System.Globalization.CultureInfo.InvariantCulture), ParameterType.GetOrPost);
                request.AddParameter(owedKey, Convert.ToString(Convert.ToDouble(user.OwedShare), System.Globalization.CultureInfo.InvariantCulture), ParameterType.GetOrPost);

                count++;
            }

            var response = await _splitwiseClient.Execute(request);
            if (response.IsSuccess)
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
                Newtonsoft.Json.Linq.JToken testToken = root["expenses"];

                var expense = JsonConvert.DeserializeObject<IEnumerable<Expense>>(testToken.ToString(), _jsonSettings);
                if (expense != null && expense.Count() != 0)
                    return expense;
            }

            Messenger.Default.Send(new ApiErrorEvent(response.StatusCode));
            return new List<Expense>();
            
        }

        public async Task<User> updateCurrentUser(User updatedUser)
        {
            var request = new RestRequest("update_user/{id}", Method.POST);
            request.AddUrlSegment("id", updatedUser.id.ToString());

            request.AddParameter("first_name", updatedUser.FirstName, ParameterType.GetOrPost);

            if (!String.IsNullOrEmpty(updatedUser.LastName))
                request.AddParameter("last_name", updatedUser.LastName, ParameterType.GetOrPost);

            request.AddParameter("email", updatedUser.Email, ParameterType.GetOrPost);

            if (!String.IsNullOrEmpty(updatedUser.DefaultCurrency))
                request.AddParameter("default_currency", updatedUser.DefaultCurrency, ParameterType.GetOrPost);

            var response = await _splitwiseClient.Execute(request);
            if (response.IsSuccess)
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
                Newtonsoft.Json.Linq.JToken testToken = root["user"];

                return JsonConvert.DeserializeObject<User>(testToken.ToString(), _jsonSettings);
            }
            else
            {
                Messenger.Default.Send(new ApiErrorEvent(response.StatusCode));
                User user = new User();
                user.FirstName = "";
                user.id = -1;
                return user;
            }
        }

        private class DeleteExpense
        {
            public Boolean success { get; set; }
            public Object error { get; set; }
        }
    }
}
