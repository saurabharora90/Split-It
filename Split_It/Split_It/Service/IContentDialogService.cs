using Split_It.Model;
using System.Threading.Tasks;

namespace Split_It.Service
{
    public interface IContentDialogService
    {
        Task showWhoPaidDialog(Expense expense);
        Task showSplitDialog(Expense expense);
    }
}
