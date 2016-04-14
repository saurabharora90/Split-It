using Microsoft.Practices.ServiceLocation;
using Split_It.Model;
using Split_It.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.Repayment
{
    public abstract class BaseRepaymentConverter : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var repayments = value as IEnumerable<Debt>;
            var user = ServiceLocator.Current.GetInstance<MainViewModel>().CurrentUser;
            Debt currentUserDebt = null;
            foreach (var debt in repayments)
            {
                if((debt.From == user.id || debt.To == user.id) && (debt.From == OtherUserId || debt.To == OtherUserId || OtherUserId == 0))
                {
                    currentUserDebt = debt;
                    break;
                }
            }

            return getValue(currentUserDebt, user);
        }



        public int OtherUserId
        {
            get { return (int)GetValue(OtherUserIdProperty); }
            set { SetValue(OtherUserIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OtherUserId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OtherUserIdProperty =
            DependencyProperty.Register("OtherUserId", typeof(int), typeof(BaseRepaymentConverter), new PropertyMetadata(0));


        protected abstract object getValue(Debt currentUserDebt, User currentUser);

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
