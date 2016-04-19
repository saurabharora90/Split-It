using Split_It.Model;
using Split_It.Utils;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Split_It.Converter.Repayment
{
    public abstract class BaseRepaymentConverter : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var repayments = value as IEnumerable<Debt>;
            var user = AppState.CurrentUser;
            Debt currentUserDebt = null;
            if (OtherUserId == 0) //this will be 0 when we are dealing with a group.
            {
                foreach (var debt in repayments)
                {
                    if ((debt.From == user.id || debt.To == user.id))
                    {
                        if (currentUserDebt == null)
                        {
                            currentUserDebt = new Debt();
                            currentUserDebt.From = debt.From;
                            currentUserDebt.To = debt.To;
                            currentUserDebt.Amount = "0";
                        }

                        double debtAmount = System.Convert.ToDouble(debt.Amount);
                        double currentUserDebtAmount = System.Convert.ToDouble(currentUserDebt.Amount);
                        if (debt.From == user.id)
                        {
                            currentUserDebtAmount -= debtAmount;
                        }
                        else if (debt.To == user.id)
                        {
                            currentUserDebtAmount += debtAmount;
                        }
                        currentUserDebt.Amount = System.Convert.ToString(currentUserDebtAmount);
                    }
                }
                if(currentUserDebt!=null)
                {
                    if(System.Convert.ToDouble(currentUserDebt.Amount) > 0)
                    {
                        currentUserDebt.To = user.id;
                        currentUserDebt.From = -1;
                    }
                    else
                    {
                        currentUserDebt.To = -1;
                        currentUserDebt.From = user.id;
                        currentUserDebt.Amount = Math.Abs(System.Convert.ToDouble(currentUserDebt.Amount)).ToString();
                    }
                }
                return getValue(currentUserDebt, user);
            }
            else //dealing with friend
            {
                foreach (var debt in repayments)
                {
                    if ((debt.From == user.id || debt.To == user.id) && (debt.From == OtherUserId || debt.To == OtherUserId))
                    {
                        currentUserDebt = debt;
                        break;
                    }
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
