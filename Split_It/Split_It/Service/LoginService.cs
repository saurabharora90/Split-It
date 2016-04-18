using RestSharp.Portable;
using RestSharp.Portable.Authenticators;
using RestSharp.Portable.WebRequest;
using Split_It.Utils;
using System;
using System.Threading.Tasks;

namespace Split_It.Service
{
    public class LoginService : ILoginService
    {
        private static String reuqestTokenURL = "get_request_token";
        private static String accessTokenURL = "get_access_token";

        private string _oAuthToken, _oAuthTokenSecret;

        public async Task<Tuple<string, string>> getAccessToken(string uri)
        {
            string oauth_veririfer = Util.GetQueryParameter(uri, "oauth_verifier");
            var client = new RestClient(Constants.SPLITWISE_API_URL);
            client.Authenticator = OAuth1Authenticator.ForAccessToken(Constants.consumerKey, Constants.consumerSecret, _oAuthToken, _oAuthTokenSecret, oauth_veririfer);
            var request = new RestRequest(accessTokenURL, Method.POST);
            var response = await client.Execute(request);
            if (response.IsSuccess)
            {
                string s = System.Text.Encoding.UTF8.GetString(response.RawBytes, 0, response.RawBytes.Length);
                string accessToken = Util.GetQueryParameter(s, "oauth_token");
                string accessTokenSecret = Util.GetQueryParameter(s, "oauth_token_secret");

                return new Tuple<string, string>(accessToken, accessTokenSecret);
            }
            return null;
        }

        public async Task<Uri> getRequestToken()
        {
            string oauth_token = await GetSplitWiseRequestTokenAsync(Constants.OAUTH_CALLBACK, Constants.consumerKey);
            return new Uri(Constants.SPLITWISE_AUTHORIZE_URL + "?oauth_token=" + oauth_token);
        }

        private async Task<string> GetSplitWiseRequestTokenAsync(string callbackUrl, string consumerKey)
        {
            var client = new RestClient(Constants.SPLITWISE_API_URL);
            client.Authenticator = OAuth1Authenticator.ForRequestToken(Constants.consumerKey, Constants.consumerSecret, Constants.OAUTH_CALLBACK);
            var request = new RestRequest(reuqestTokenURL, Method.POST);
            var response = await client.Execute(request);
            if (response.IsSuccess)
            {
                string s = System.Text.Encoding.UTF8.GetString(response.RawBytes, 0, response.RawBytes.Length);
                _oAuthToken = Util.GetQueryParameter(s, "oauth_token");
                _oAuthTokenSecret = Util.GetQueryParameter(s, "oauth_token_secret");

                return _oAuthToken;
            }
            return null;
        }
    }
}
