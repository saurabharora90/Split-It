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
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Friend>> getFriendsList()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Group>> getGroupsList()
        {
            throw new NotImplementedException();
        }
    }
}
