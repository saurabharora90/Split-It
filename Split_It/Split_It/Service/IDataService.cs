using Split_It.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Split_It.Service
{
    public interface IDataService
    {
        Task<User> getCurrentUser();
        Task<IEnumerable<Friend>> getFriendsList();
        Task<IEnumerable<Group>> getGroupsList();
    }
}
