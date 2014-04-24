using RestSharp;
using RestSharp.Authenticators;
using Split_It_.Model;
using Split_It_.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void getAllExpenses(Action<List<Expense>> CallbackOnSuccess)
        {
            var request = new RestRequest(getExpensesURL);
            request.AddParameter("limit", 0, ParameterType.GetOrPost);
            request.RootElement = "expenses";
            client.ExecuteAsync<List<Expense>>(request, reponse =>
                {
                    List<Expense> expenses = reponse.Data;
                    CallbackOnSuccess(expenses);
                });
        }
    }
}
