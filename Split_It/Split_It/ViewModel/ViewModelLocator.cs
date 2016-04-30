using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Split_It.Service;

namespace Split_It.ViewModel
{
    public class ViewModelLocator
    {
        public const string MainPageKey = "MainPage";
        public const string FriendDetailPageKey = "FriendDetailPage";
        public const string ExpenseDetailPageKey = "ExpenseDetailPage";
        public const string GroupDetailPageKey = "GroupDetailPage";
        public const string AddExpensePageKey = "AddExpensePage";

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var nav = new NavigationService();
            nav.Configure(MainPageKey, typeof(MainPage));
            nav.Configure(FriendDetailPageKey, typeof(FriendDetailPage));
            nav.Configure(ExpenseDetailPageKey, typeof(ExpenseDetailPage));
            nav.Configure(GroupDetailPageKey, typeof(GroupDetailPage));
            nav.Configure(AddExpensePageKey, typeof(AddExpensePage));
            SimpleIoc.Default.Register<INavigationService>(() => nav);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                //SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
                SimpleIoc.Default.Register<IDataService, DataService>();
            }

            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<ILoginService, LoginService>();
            SimpleIoc.Default.Register<IWebviewService, WebviewService>();
            SimpleIoc.Default.Register<IContentDialogService, ContentDialogService>();

            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<FriendDetailViewModel>(); ;
            SimpleIoc.Default.Register<GroupDetailViewModel>();
            SimpleIoc.Default.Register<ExpenseDetailViewModel>();
            SimpleIoc.Default.Register<AddExpenseViewModel>();
            SimpleIoc.Default.Register<UserAccountViewModel>();

            SimpleIoc.Default.Register<WhoPaidDialogViewModel>();
            SimpleIoc.Default.Register<IOUDialogViewModel>();
            SimpleIoc.Default.Register<SplitDialogViewModel>();

            SimpleIoc.Default.Register<SideMenuViewModel>();
        }

        /// <summary>
        /// Gets the MainVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel MainVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        /// <summary>
        /// Gets the LoginVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public LoginViewModel LoginVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }
        }

        /// <summary>
        /// Gets the FriendDetailVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public FriendDetailViewModel FriendDetailVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<FriendDetailViewModel>();
            }
        }

        /// <summary>
        /// Gets the ExpenseDetailVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ExpenseDetailViewModel ExpenseDetailVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ExpenseDetailViewModel>();
            }
        }

        /// <summary>
        /// Gets the GroupDetailVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public GroupDetailViewModel GroupDetailVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<GroupDetailViewModel>();
            }
        }

        /// <summary>
        /// Gets the AddExpenseVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AddExpenseViewModel AddExpenseVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddExpenseViewModel>();
            }
        }

        /// <summary>
        /// Gets the WhoPaidDialogVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public WhoPaidDialogViewModel WhoPaidDialogVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WhoPaidDialogViewModel>();
            }
        }
        /// <summary>
        /// Gets the IOUDialogVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public IOUDialogViewModel IOUDialogVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IOUDialogViewModel>();
            }
        }

        /// <summary>
        /// Gets the SplitDialogVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SplitDialogViewModel SplitDialogVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SplitDialogViewModel>();
            }
        }

        /// <summary>
        /// Gets the SideMenuVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SideMenuViewModel SideMenuVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SideMenuViewModel>();
            }
        }
        
        /// <summary>
        /// Gets the UserAccountVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public UserAccountViewModel UserAccountVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UserAccountViewModel>();
            }
        }
    }
}
