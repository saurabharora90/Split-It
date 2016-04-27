using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Split_It.Service
{
    public class WebviewService : IWebviewService
    {
        public WebView MyWebView { get; set; }

        public void NavigateTo(Uri uri)
        {
            if(MyWebView!=null)
                MyWebView.Navigate(uri);
        }
    }
}
