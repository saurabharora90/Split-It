using Split_It.Utils;

namespace Split_It.Model.Enum
{
    public enum FriendFilter
    {
        [Display("All")]
        All,
        [Display("You owe")]
        YouOwe,
        [Display("You are owed")]
        OwesYou,
        [Display("Recent")]
        Recent
    }
}
