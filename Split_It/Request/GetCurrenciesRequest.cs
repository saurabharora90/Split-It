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
            request.RootElement = "currencies";
            client.ExecuteAsync<List<Currency>>(request, reponse =>
            {
                List<Currency> supportedCurrencies = reponse.Data;
                CallbackOnSuccess(supportedCurrencies);
            });
        }
    }
}
