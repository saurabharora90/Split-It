using System;
using System.Threading.Tasks;
using Split_It.Model;
using Split_It.Dialog;

namespace Split_It.Service
{
    public class ContentDialogService : IContentDialogService
    {
        public async Task showIOUDialog(Expense expense)
        {
            IOUDialog dialog = new IOUDialog(expense);
            await dialog.ShowAsync();
        }

        public Task showSplitDialog(Expense expense)
        {
            throw new NotImplementedException();
        }

        public async Task showWhoPaidDialog(Expense expense)
        {
            WhoPaidDialog dialog = new WhoPaidDialog(expense);
            await dialog.ShowAsync();
        }
    }
}
