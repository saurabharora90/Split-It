using Split_It.Model;
using Split_It.ViewModel;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Split_It
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GroupDetailPage : Page
    {
        static string _positionKey;

        public GroupDetailPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ((GroupDetailViewModel)DataContext).CurrentGroup = e.Parameter as Group;
            ((GroupDetailViewModel)DataContext).RegisterMessengerCommand.Execute(null);

            if (_positionKey == null) return;
            await ListViewPersistenceHelper.SetRelativeScrollPositionAsync(myListView, _positionKey, KeyToItemHandler);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                ((GroupDetailViewModel)DataContext).Cleanup();
                _positionKey = null;
            }
            else
                _positionKey = ListViewPersistenceHelper.GetRelativeScrollPosition(myListView, ItemToKeyHandler);
            base.OnNavigatedFrom(e);
        }

        private IAsyncOperation<object> KeyToItemHandler(string key)
        {
            Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<object>> taskProvider = token =>
            {
                var viewModel = DataContext as BaseEntityDetailViewModel;
                if (viewModel == null) return null;
                foreach (var item in viewModel.ExpensesList)
                {
                    if (item.Id == Convert.ToInt32(key)) return Task.FromResult(item as object);
                }
                return Task.FromResult((object)null);
            };

            return AsyncInfo.Run(taskProvider);
        }

        private string ItemToKeyHandler(object item)
        {
            Expense dataItem = item as Expense;
            if (dataItem == null) return null;

            return dataItem.Id.ToString();
        }
    }
}
