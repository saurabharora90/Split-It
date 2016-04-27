using Split_It.Model;
using Split_It.ViewModel;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Split_It.Dialog
{
    public sealed partial class IOUDialog : ContentDialog
    {
        public IOUDialog(Expense expense)
        {
            this.InitializeComponent();
            var dataContext = ((IOUDialogViewModel)(DataContext));
            dataContext.CurrentExpense = expense;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Hide();
        }
    }
}
