using Split_It.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Split_It.Model;

namespace Split_It.Design
{
    public class DesignDataService : IDataService
    {
        public Task<User> getCurrentUser()
        {
            return Task.FromResult(new User { FirstName = "Saurabh", LastName = "Arora", Picture = new Photo { Medium = "https://graph.facebook.com/598294269/picture?type=normal" } });
        }

        public Task<IEnumerable<Friend>> getFriendsList()
        {
            List<Friend> friendsList = new List<Friend>(4);
            Photo picture = new Photo { Medium = "https://graph.facebook.com/598294269/picture?type=normal" };
            UserBalance noBalance = new UserBalance { Amount = "-0.0", CurrencyCode = "SGD" };
            UserBalance sgdBalance = new UserBalance { Amount = "-12.0", CurrencyCode = "SGD" };
            UserBalance thbBalance = new UserBalance { Amount = "25.0", CurrencyCode = "THB" };

            friendsList.Add(new Friend { FirstName = "No Balance", Picture = picture, Balance = new List<UserBalance> { noBalance } });
            friendsList.Add(new Friend { FirstName = "I Owe", Picture = picture, Balance = new List<UserBalance> { sgdBalance } });
            friendsList.Add(new Friend { FirstName = "She owes", Picture = picture, Balance = new List<UserBalance> { thbBalance } });
            friendsList.Add(new Friend { FirstName = "Multiple", Picture = picture, Balance = new List<UserBalance> { thbBalance, sgdBalance } });

            return Task.FromResult(friendsList.AsEnumerable());
        }

        public Task<IEnumerable<Group>> getGroupsList()
        {
            return null;
        }
    }
}
