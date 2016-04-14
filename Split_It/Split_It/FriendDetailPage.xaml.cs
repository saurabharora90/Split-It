using Split_It.Model;
using Split_It.ViewModel;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Split_It
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FriendDetailPage : Page
    {
        public FriendDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
                return;
            Tuple<int, Friend> dict = e.Parameter as Tuple<int, Friend>;
            if (dict == null)
                return;
            ((FriendDetailViewModel)DataContext).FriendshipId = dict.Item1;
            ((FriendDetailViewModel)DataContext).CurrentFriend = dict.Item2;
            ((FriendDetailViewModel)DataContext).RegisterMessengerCommand.Execute(null);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                ((FriendDetailViewModel)DataContext).Cleanup();
            base.OnNavigatedFrom(e);
        }
    }
}
