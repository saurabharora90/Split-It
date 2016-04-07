using RestSharp.Portable;
using RestSharp.Portable.Authenticators;
using RestSharp.Portable.HttpClient;
using Split_It.Model;
using Split_It.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Split_It.Service
{
    public class DataService : IDataService
    {
        private RestClient _splitwiseClient;
        private JsonSerializerSettings _jsonSettings;

        public DataService()
        {
            _splitwiseClient = new RestClient(Constants.SPLITWISE_API_URL);
            _splitwiseClient.Authenticator = OAuth1Authenticator.ForProtectedResource(Constants.consumerKey, Constants.consumerSecret, AppState.AccessToken, AppState.AccessTokenSecret);

            _jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new DeliminatorSeparatedPropertyNamesContractResolver('_') };
        }

        private string getStringFromResponse(IRestResponse response)
        {
            return System.Text.Encoding.UTF8.GetString(response.RawBytes, 0, response.RawBytes.Length);
        }

        public async Task<User> getCurrentUser()
        {
            var request = new RestRequest("get_current_user");
            var response = await _splitwiseClient.Execute(request);
            Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
            Newtonsoft.Json.Linq.JToken testToken = root["user"];

            return JsonConvert.DeserializeObject<User>(testToken.ToString(), _jsonSettings);
        }

        public async Task<IEnumerable<Friend>> getFriendsList()
        {
            var request = new RestRequest("get_friends");
            var response = await _splitwiseClient.Execute(request);
            Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
            Newtonsoft.Json.Linq.JToken testToken = root["friends"];

            return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Friend>>(testToken.ToString(), _jsonSettings);
        }

        public async Task<IEnumerable<Group>> getGroupsList()
        {
            var request = new RestRequest("get_groups");
            var response = await _splitwiseClient.Execute(request);
            Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(getStringFromResponse(response));
            Newtonsoft.Json.Linq.JToken testToken = root["groups"];

            return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Group>>(testToken.ToString(), _jsonSettings);
        }

    }
}
