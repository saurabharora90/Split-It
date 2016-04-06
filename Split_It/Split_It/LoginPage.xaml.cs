using Microsoft.Practices.ServiceLocation;
using Split_It.Service;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Split_It
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
            ServiceLocator.Current.GetInstance<IWebviewService>().MyWebView = MyWebView;
        }
    }
}
