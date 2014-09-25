using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Model
{
    public class AmountSplit
    {
        const String SPLIT_EQUALLY_STRING = "Split equally";
        const String YOU_OWE_STRING = "You owe";
        const String FRIEND_OWES_STRING = "Friend owes";
        const String SPLIT_UNEQUALLY_STRING = "Split unequally";

        public const int SPLIT_EQUALLY = 0;
        public const int YOU_OWE = 1;
        public const int FRIEND_OWES = 2;
        public const int SPLIT_UNEQUALLY = 3;

        public int id { get; set; }
        public String name { get; set; }

        public static AmountSplit EqualSplit = new AmountSplit() { id = SPLIT_EQUALLY, name = SPLIT_EQUALLY_STRING };
        public static AmountSplit YouOwe = new AmountSplit() { id = YOU_OWE, name = YOU_OWE_STRING };
        public static AmountSplit FriendOwes = new AmountSplit() { id = FRIEND_OWES, name = FRIEND_OWES_STRING };
        public static AmountSplit UnequalSplit = new AmountSplit() { id = SPLIT_UNEQUALLY, name = SPLIT_UNEQUALLY_STRING };

        public static ObservableCollection<AmountSplit> GetTwoFriendsSplitMethodList()
        {
            ObservableCollection<AmountSplit> splitList = new ObservableCollection<AmountSplit>();
            splitList.Add(EqualSplit);
            splitList.Add(YouOwe);
            splitList.Add(FriendOwes);
            splitList.Add(UnequalSplit);
            return splitList;
        }

        public static ObservableCollection<AmountSplit> GetMoreThanTwoFriendsSplitMethodList()
        {
            ObservableCollection<AmountSplit> splitList = new ObservableCollection<AmountSplit>();
            splitList.Add(EqualSplit);
            splitList.Add(UnequalSplit);
            return splitList;
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            AmountSplit p = obj as AmountSplit;
            if ((System.Object)p == null)
            {
                return false;
            }

            return p.id == id;
        }

        public override int GetHashCode()
        {
            return id;
        }
    }
}
