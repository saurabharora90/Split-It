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
            return null;
        }

        public Task<IEnumerable<Group>> getGroupsList()
        {
            return null;
        }
    }
}
