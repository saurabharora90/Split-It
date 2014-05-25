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
    class AddExpenseRequest : BaseRequest
    {
        public static String addExpenseURL = "create_expense";
        Expense paymentExpense;

        public AddExpenseRequest(Expense expense)
            : base()
        {
            this.paymentExpense = expense;
        }

        public void addExpense(Action<bool> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(addExpenseURL, Method.POST);
            request.RootElement = "expenses";

            if(paymentExpense.payment)
                request.AddParameter("payment", "true", ParameterType.GetOrPost);
            else
                request.AddParameter("payment", "false", ParameterType.GetOrPost);

            request.AddParameter("cost", paymentExpense.cost, ParameterType.GetOrPost);
            request.AddParameter("description", paymentExpense.description, ParameterType.GetOrPost);
            request.AddParameter("currency_code", paymentExpense.currency_code, ParameterType.GetOrPost);
            request.AddParameter("creation_method", paymentExpense.creation_method, ParameterType.GetOrPost);
            
            if (!String.IsNullOrEmpty(paymentExpense.details))
            {
                request.AddParameter("details", paymentExpense.details, ParameterType.GetOrPost);
            }

            if (!String.IsNullOrEmpty(paymentExpense.date))
            {
                request.AddParameter("date", paymentExpense.date, ParameterType.GetOrPost);
            }

            if (paymentExpense.group_id!=0)
            {
                request.AddParameter("group_id", paymentExpense.group_id, ParameterType.GetOrPost);
            }

            int count = 0;
            foreach (var user in paymentExpense.users)
            {
                string idKey = String.Format("users__array_{0}__user_id", count);
                string paidKey = String.Format("users__array_{0}__paid_share", count);
                string owedKey = String.Format("users__array_{0}__owed_share", count);
                request.AddParameter(idKey, user.user_id, ParameterType.GetOrPost);
                request.AddParameter(paidKey, user.paid_share, ParameterType.GetOrPost);
                request.AddParameter(owedKey, user.owed_share, ParameterType.GetOrPost);

                count++;
            }


            client.ExecuteAsync<List<Expense>>(request, reponse =>
                {
                    List<Expense> expenseList = reponse.Data;
                    if (expenseList != null)
                    {
                        Expense payment = expenseList[0];
                        if (payment.id!=0)
                            CallbackOnSuccess(true);
                        else
                            CallbackOnFailure(reponse.StatusCode);
                    }
                    else
                        CallbackOnFailure(reponse.StatusCode);
                });
        }
    }
}