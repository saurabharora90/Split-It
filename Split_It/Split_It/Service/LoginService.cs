using RestSharp.Portable;
using RestSharp.Portable.Authenticators;
using RestSharp.Portable.HttpClient;
using Split_It.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace Split_It.Service
{
    public class LoginService : ILoginService
    {
        private static String reuqestTokenURL = "get_request_token";
        private static String accessTokenURL = "get_access_token";

        private string _oAuthToken, _oAuthTokenSecret;

        public Task<Tuple<string, string>> getAccessToken(string param)
        {
            throw new NotImplementedException();
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
                String[] Tokens = s.Split('&');

                for (int i = 0; i < Tokens.Length; i++)
                {
                    String[] splits = Tokens[i].Split('=');
                    switch (splits[0])
                    {
                        case "oauth_token":
                            _oAuthToken = splits[1];
                            break;
                        case "oauth_token_secret":
                            _oAuthTokenSecret = splits[1];
                            break;
                    }
                }

                return _oAuthToken;
            }
            return null;
        }
    }
}
