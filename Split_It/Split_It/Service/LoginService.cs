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

namespace Split_It.Service
{
    public class LoginService : ILoginService
    {
        private static String reuqestTokenURL = "get_request_token";
        private static String accessTokenURL = "get_access_token";

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
            // Acquiring a request token
            string SplitwiseUrl = Constants.SPLITWISE_API_URL + reuqestTokenURL;

            string nonce = GetNonce();
            string timeStamp = GetTimeStamp();
            string SigBaseStringParams = "oauth_callback=" + Uri.EscapeDataString(callbackUrl);
            SigBaseStringParams += "&" + "oauth_consumer_key=" + consumerKey;
            SigBaseStringParams += "&" + "oauth_nonce=" + nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            string SigBaseString = "GET&";
            SigBaseString += Uri.EscapeDataString(SplitwiseUrl) + "&" + Uri.EscapeDataString(SigBaseStringParams);
            string Signature = GetSignature(SigBaseString, Constants.consumerSecret);

            SplitwiseUrl += "?" + SigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(Signature);
            HttpClient httpClient = new HttpClient();
            string GetResponse = await httpClient.GetStringAsync(new Uri(SplitwiseUrl));

            string request_token = null;
            string oauth_token_secret = null;
            string[] keyValPairs = GetResponse.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                string[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        request_token = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauth_token_secret = splits[1];
                        break;
                }
            }

            return request_token;
        }

        string GetNonce()
        {
            Random rand = new Random();
            int nonce = rand.Next(1000000000);
            return nonce.ToString();
        }

        string GetTimeStamp()
        {
            TimeSpan SinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return Math.Round(SinceEpoch.TotalSeconds).ToString();
        }

        string GetSignature(string sigBaseString, string consumerSecretKey)
        {
            IBuffer KeyMaterial = CryptographicBuffer.ConvertStringToBinary(consumerSecretKey + "&", BinaryStringEncoding.Utf8);
            MacAlgorithmProvider HmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            CryptographicKey MacKey = HmacSha1Provider.CreateKey(KeyMaterial);
            IBuffer DataToBeSigned = CryptographicBuffer.ConvertStringToBinary(sigBaseString, BinaryStringEncoding.Utf8);
            IBuffer SignatureBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned);
            string Signature = CryptographicBuffer.EncodeToBase64String(SignatureBuffer);

            return Signature;
        }
    }
}
