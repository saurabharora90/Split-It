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
    class GetCommentsRequest : BaseRequest
    {
        public static String getCommentsURL = "get_comments";
        private int expenseId;

        public GetCommentsRequest(int expenseId)
            : base()
        {
            this.expenseId = expenseId;
        }

        public void getComments(Action<List<Comment>> Callback)
        {
            var request = new RestRequest(getCommentsURL);
            request.AddParameter("expense_id", expenseId, ParameterType.GetOrPost);
            client.ExecuteAsync(request, reponse =>
                {
                    if(reponse.StatusCode != HttpStatusCode.OK && reponse.StatusCode != HttpStatusCode.NotModified)
                    {
                        Callback(null);
                        return;
                    }
                    Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(reponse.Content);
                    Newtonsoft.Json.Linq.JToken testToken = root["comments"];
                    JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                    List<Comment> comments = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Comment>>(testToken.ToString(), settings);
                    Callback(comments);
                });
        }
    }
}
