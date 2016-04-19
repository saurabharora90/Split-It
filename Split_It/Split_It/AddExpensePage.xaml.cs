using Split_It.Model;
using Split_It.ViewModel;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Split_It
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddExpensePage : Page
    {
        IEnumerable<Friend> allFriends;

        public AddExpensePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var dataContext = ((AddExpenseViewModel)(DataContext));
            if (e.Parameter is Expense)
                dataContext.ExpenseToAdd = e.Parameter as Expense;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                ((AddExpenseViewModel)(DataContext)).Cleanup();
            }
            base.OnNavigatedFrom(e);
        }

        private void friendsBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if(allFriends == null)
            {
                allFriends = friendsBox.ItemsSource as IEnumerable<Friend>;
            }

            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var filter = allFriends.Where(p => p.Name.ToUpper().Contains(sender.Text.ToUpper()));
                sender.ItemsSource = filter;
            }
        }

        private void friendsBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var chosenFriend = args.SelectedItem as Friend;
            ((AddExpenseViewModel)(DataContext)).AddUserCommand.Execute((new ExpenseUser() { User = chosenFriend, UserId = chosenFriend.id }));
            sender.Text = String.Empty;
        }
    }
}
