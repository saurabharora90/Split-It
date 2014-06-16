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
    class CreateCommentRequest : BaseRequest
    {
        public static String createCommentURL = "create_comment";
        private int expenseId;
        private string content;

        public CreateCommentRequest(int expenseId, string content)
            : base()
        {
            this.expenseId = expenseId;
            this.content = content;
        }

        public void postComment(Action<List<Comment>> Callback)
        {
            var request = new RestRequest(createCommentURL);
            request.AddParameter("expense_id", expenseId, ParameterType.GetOrPost);
            request.AddParameter("content", content, ParameterType.GetOrPost);
            request.RootElement = "comment";
            client.ExecuteAsync<List<Comment>>(request, reponse =>
                {
                    if(reponse.StatusCode != HttpStatusCode.OK && reponse.StatusCode != HttpStatusCode.NotModified)
                    {
                        Callback(null);
                        return;
                    }
                    List<Comment> comments = reponse.Data;
                    Callback(comments);
                });
        }
    }
}
