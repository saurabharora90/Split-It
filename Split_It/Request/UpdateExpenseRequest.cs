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
    class UpdateExpenseRequest : BaseRequest
    {
        public static String updateExpenseURL = "update_expense/{id}";
        Expense updatedExpense;

        public UpdateExpenseRequest(Expense expense)
            : base()
        {
            this.updatedExpense = expense;
        }

        public void updateExpense(Action<bool> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(updateExpenseURL, Method.POST);
            request.AddUrlSegment("id", updatedExpense.id.ToString());

            if(updatedExpense.payment)
                request.AddParameter("payment", "true", ParameterType.GetOrPost);
            else
                request.AddParameter("payment", "false", ParameterType.GetOrPost);

            request.AddParameter("cost", updatedExpense.cost, ParameterType.GetOrPost);
            request.AddParameter("description", updatedExpense.description, ParameterType.GetOrPost);

            if (!String.IsNullOrEmpty(updatedExpense.currency_code))
                request.AddParameter("currency_code", updatedExpense.currency_code, ParameterType.GetOrPost);

            if (!String.IsNullOrEmpty(updatedExpense.creation_method))
            {
                request.AddParameter("creation_method", updatedExpense.creation_method, ParameterType.GetOrPost);
            }
            
            if (!String.IsNullOrEmpty(updatedExpense.details) && !updatedExpense.details.Equals(Expense.DEFAULT_DETAILS))
            {
                request.AddParameter("details", updatedExpense.details, ParameterType.GetOrPost);
            }

            if (!String.IsNullOrEmpty(updatedExpense.date))
            {
                request.AddParameter("date", updatedExpense.date, ParameterType.GetOrPost);
            }

            if (updatedExpense.group_id!=0)
            {
                request.AddParameter("group_id", updatedExpense.group_id, ParameterType.GetOrPost);
            }

            int count = 0;
            foreach (var user in updatedExpense.users)
            {
                string idKey = String.Format("users__array_{0}__user_id", count);
                string paidKey = String.Format("users__array_{0}__paid_share", count);
                string owedKey = String.Format("users__array_{0}__owed_share", count);
                request.AddParameter(idKey, user.user_id, ParameterType.GetOrPost);
                request.AddParameter(paidKey, user.paid_share, ParameterType.GetOrPost);
                request.AddParameter(owedKey, user.owed_share, ParameterType.GetOrPost);

                count++;
            }

            client.ExecuteAsync(request, reponse =>
                {
                    try
                    {
                        Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(reponse.Content);
                        Newtonsoft.Json.Linq.JToken testToken = root["expenses"];
                        JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                        List<Expense> expenseList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Expense>>(testToken.ToString(), settings);
                        if (expenseList != null)
                        {
                            Expense payment = expenseList[0];
                            if (payment.id != 0)
                                CallbackOnSuccess(true);
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
