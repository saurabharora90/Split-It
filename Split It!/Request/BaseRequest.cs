using RestSharp;
using RestSharp.Authenticators;
using Split_It_.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Request
{
    abstract class BaseRequest
    {
        protected RestClient client;

        public BaseRequest()
        {
            client = new RestClient(Constants.SPLITWISE_API_URL);
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(Constants.consumerKey, Constants.consumerSecret, App.accessToken, App.accessTokenSecret);
        }
    }
}
