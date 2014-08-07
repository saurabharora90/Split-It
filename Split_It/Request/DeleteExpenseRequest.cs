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
    class DeleteExpenseRequest : BaseRequest
    {
        public static String deleteExpenseURL = "delete_expense/{id}";
        private int expenseId;

        public DeleteExpenseRequest(int id)
            : base()
        {
            expenseId = id;
        }

        public void deleteExpense(Action<bool> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(deleteExpenseURL, Method.POST);
            request.AddUrlSegment("id", expenseId.ToString());
            client.ExecuteAsync(request, reponse =>
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                    DeleteExpense result = Newtonsoft.Json.JsonConvert.DeserializeObject<DeleteExpense>(reponse.Content, settings);
                    if (result.success)
                        CallbackOnSuccess(result.success);
                    else
                        CallbackOnFailure(reponse.StatusCode);
                });
        }

        private class DeleteExpense
        {
            public Boolean success { get; set; }
            public Object error { get; set; }
        }
    }    
}
