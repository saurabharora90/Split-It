using Split_It.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Split_It.Model;
using System.IO;
using Newtonsoft.Json;
using Split_It.Utils;

namespace Split_It.Design
{
    public class DesignDataService : IDataService
    {
        private JsonSerializerSettings _jsonSettings;

        public DesignDataService()
        {
            _jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new DeliminatorSeparatedPropertyNamesContractResolver('_') };
        }

        public Task<User> getCurrentUser()
        {
            return Task.FromResult(new User { id = 569955, FirstName = "Saurabh", LastName = "Arora", DefaultCurrency = "SGD", Picture = new Photo { Medium = "https://graph.facebook.com/598294269/picture?type=normal" } });
        }

        public Task<IEnumerable<Friend>> getFriendsList()
        {
            List<Friend> friendsList = new List<Friend>(4);
            Photo picture = new Photo { Medium = "https://graph.facebook.com/598294269/picture?type=normal" };
            UserBalance noBalance = new UserBalance { Amount = "-0.0", CurrencyCode = "SGD" };
            UserBalance sgdBalance = new UserBalance { Amount = "-12.25", CurrencyCode = "SGD" };
            UserBalance thbBalance = new UserBalance { Amount = "25.0", CurrencyCode = "THB" };

            friendsList.Add(new Friend { FirstName = "No Balance", Picture = picture, Balance = new List<UserBalance> { noBalance } });
            friendsList.Add(new Friend { FirstName = "I Owe", Picture = picture, Balance = new List<UserBalance> { sgdBalance } });
            friendsList.Add(new Friend { FirstName = "She owes", Picture = picture, Balance = new List<UserBalance> { thbBalance } });
            friendsList.Add(new Friend { FirstName = "Multiple", Picture = picture, Balance = new List<UserBalance> { thbBalance, sgdBalance } });

            return Task.FromResult(friendsList.AsEnumerable());
        }

        public Task<IEnumerable<Group>> getGroupsList()
        {
            string text = File.ReadAllText("Data/sample_group.json");
            Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(text);
            Newtonsoft.Json.Linq.JToken testToken = root["groups"];
            return Task.FromResult(JsonConvert.DeserializeObject<IEnumerable<Group>>(testToken.ToString(), _jsonSettings));
        }
    }
}
