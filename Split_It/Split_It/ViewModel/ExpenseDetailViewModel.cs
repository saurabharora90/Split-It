using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Split_It.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.ViewModel
{
    public class ExpenseDetailViewModel : ViewModelBase
    {
        protected IDataService _dataService;
        protected INavigationService _navigationService;

        public ExpenseDetailViewModel(IDataService dataService, INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
        }
    }
}
