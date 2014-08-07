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
    class GetCurrenciesRequest : BaseRequest
    {
        public static String getCurrenciesURL = "get_currencies";

        public GetCurrenciesRequest()
            : base()
        {
        }

        public void getSupportedCurrencies(Action<List<Currency>> CallbackOnSuccess)
        {
            var request = new RestRequest(getCurrenciesURL);
            client.ExecuteAsync(request, reponse =>
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(reponse.Content);
                Newtonsoft.Json.Linq.JToken testToken = root["currencies"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                List<Currency> supportedCurrencies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Currency>>(testToken.ToString(), settings);
                CallbackOnSuccess(supportedCurrencies);
            });
        }
    }
}
