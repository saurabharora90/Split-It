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
    class RecordPaymentRequesr : BaseRequest
    {
        public static String deleteExpenseURL = "create_expense";
        private int toUserId;
        private double amount;
        private string description, currencyCode, date;

        public RecordPaymentRequesr(int userId, double amount, string desc, string currency, string date)
            : base()
        {
            this.toUserId = userId;
            this.amount = amount;
            this.description = desc;
            this.currencyCode = currency;
            this.date = date;
        }

        public void deleteExpense(Action<bool> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(deleteExpenseURL, Method.POST);
            //request.AddParameter("limit", 0, ParameterType.GetOrPost);
            client.ExecuteAsync<Boolean>(request, reponse =>
                {
                    //somehow this API returns false when the expense has been deleted
                    if (!reponse.Data)
                        CallbackOnSuccess(reponse.Data);
                    else
                        CallbackOnFailure(reponse.StatusCode);
                });
        }
    }
}
