using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Service
{
    public interface ILoginService
    {
        /// <summary>
        /// Returns the URI to navigate to
        /// </summary>
        /// <returns></returns>
        Task<Uri> getRequestToken();

        /// <summary>
        /// Returns the accessToken and accessTokenSecret
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<Tuple<string, string>> getAccessToken(string param);
    }
}
