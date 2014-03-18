using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Split_It_.Utils;
using RestSharp;
using RestSharp.Authenticators;
using System.Windows.Threading;

namespace Split_It_.Request
{
    class OAuthRequest
    {
        public static String reuqestTokenURL = "get_request_token";
        public static String accessTokenURL = "get_access_token";
        public static String consumerKey = "wQukgHsUKubX5gal5rVKFXJ48lfvhxTXL9H12SkE";
        public static String consumerSecret = "tt9wZlnI3MKOsvW3bKlrFflCEM5MvKtrOV4Ba8U8";

        private string _oAuthToken, _oAuthTokenSecret;

        public void getReuqestToken(Action<Uri> CallbackOnSuccess)
        {
            var client = new RestClient(Constants.SPLITWISE_API_URL);
            client.Authenticator = OAuth1Authenticator.ForRequestToken(consumerKey, consumerSecret, Constants.OAUTH_CALLBACK);
            var request = new RestRequest(OAuthRequest.reuqestTokenURL, Method.POST);
            client.ExecuteAsync(request, response =>
            {
                if (response.ResponseStatus != ResponseStatus.Error)
                {
                    _oAuthToken = Util.GetQueryParameter(response.Content, "oauth_token");
                    _oAuthTokenSecret = Util.GetQueryParameter(response.Content, "oauth_token_secret");
                    String authorizeUrl = Constants.SPLITWISE_AUTHORIZE_URL + "?oauth_token=" + _oAuthToken;
                    CallbackOnSuccess(new Uri(authorizeUrl));
                }
            });

        }

        public void getAccessToken(string uri, Action<String, String> CallbackOnSuccess)
        {
            string oauth_veririfer = Util.GetQueryParameter(uri, "oauth_verifier");
            var client = new RestClient(Constants.SPLITWISE_API_URL);
            client.Authenticator = OAuth1Authenticator.ForAccessToken(consumerKey, consumerSecret, _oAuthToken, _oAuthTokenSecret, oauth_veririfer);
            var request = new RestRequest(OAuthRequest.accessTokenURL, Method.POST);
            client.ExecuteAsync(request, response =>
            {
                string accessToken = Util.GetQueryParameter(response.Content, "oauth_token");
                string accessTokenSecret = Util.GetQueryParameter(response.Content, "oauth_token_secret");
                CallbackOnSuccess(accessToken, accessTokenSecret);
            });
        }
    }
}
