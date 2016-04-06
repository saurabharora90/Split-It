using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Split_It.Service
{
    public interface IWebviewService
    {
        WebView MyWebView { get; set; }

        void NavigateTo(Uri uri);
    }
}
