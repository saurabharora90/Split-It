using Split_It.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Split_It.Service
{
    public interface IDataService
    {
        Task<User> getCurrentUser();
        Task<IEnumerable<Friend>> getFriendsList();
        Task<IEnumerable<Friendship>> getFriendShip();
        Task<IEnumerable<Group>> getGroupsList();
        Task<IEnumerable<Expense>> getExpenseForFriend(int friendshipId, int limit, int offset = 0);
        Task<IEnumerable<Expense>> getExpenseForGroup(int groupId, int limit, int offset = 0);

        Task<bool> deleteExpense(int expenseId);
        Task<Friend> getFriendInfo(int friendId);
    }
}
