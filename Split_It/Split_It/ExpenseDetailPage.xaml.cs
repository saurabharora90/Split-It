using Microsoft.Practices.ServiceLocation;
using Split_It.Model;
using Split_It.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Split_It
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExpenseDetailPage : Page
    {
        public static int TYPE_GROUP = 1000;
        public static int TYPE_FRIEND = 1001;

        public ExpenseDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var expense = e.Parameter as Expense;
            if(expense!=null)
            {
                var VM = ServiceLocator.Current.GetInstance<ExpenseDetailViewModel>();
                VM.SelectedExpense = expense;
                DataContext = VM;
            }
            else
            {
                int type = (int)e.Parameter;
                if(type == TYPE_FRIEND)
                {
                    var VM = ServiceLocator.Current.GetInstance<FriendDetailViewModel>();
                    DataContext = VM;
                }

            }
            base.OnNavigatedTo(e);
        }
    }
}
