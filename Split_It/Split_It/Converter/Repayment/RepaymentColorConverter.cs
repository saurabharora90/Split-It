using Split_It.Model;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;

namespace Split_It.Converter.Repayment
{
    public class RepaymentColorConverter : BaseRepaymentConverter
    {
        protected override object getValue(Debt debt, User currentUser)
        {
            var brush = Application.Current.Resources["settled"] as SolidColorBrush;
            if (debt == null)
                return brush;

            if (currentUser.id == debt.From)
            {
                brush = Application.Current.Resources["negative"] as SolidColorBrush; ;
            }
            else if (currentUser.id == debt.To)
            {
                brush = Application.Current.Resources["positive"] as SolidColorBrush; ;
            }

            return brush;
        }
    }
}
