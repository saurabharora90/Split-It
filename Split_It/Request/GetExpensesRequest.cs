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
    class GetExpensesRequest : BaseRequest
    {
        public static String getExpensesURL = "get_expenses";

        public GetExpensesRequest()
            : base()
        {
        }

        public void getAllExpenses(Action<List<Expense>> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(getExpensesURL);
            request.AddParameter("limit", 0, ParameterType.GetOrPost);
            request.AddParameter("updated_after", Util.getLastUpdatedTime(), ParameterType.GetOrPost);
            request.RootElement = "expenses";
            try
            {
                client.ExecuteAsync<List<Expense>>(request, reponse =>
                    {
                        if (reponse.StatusCode != HttpStatusCode.OK && reponse.StatusCode != HttpStatusCode.NotModified)
                        {
                            CallbackOnFailure(reponse.StatusCode);
                            return;
                        }
                        List<Expense> expenses = reponse.Data;
                        CallbackOnSuccess(expenses);
                    });
            }
            catch (Exception e)
            {
                CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
