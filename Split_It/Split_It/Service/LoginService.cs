using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Service
{
    public class LoginService : ILoginService
    {
        private static String reuqestTokenURL = "get_request_token";
        private static String accessTokenURL = "get_access_token";

        public Task<Tuple<string, string>> getAccessToken(string param)
        {
            throw new NotImplementedException();
        }

        public Task<Uri> getRequestToken()
        {
            throw new NotImplementedException();
        }
    }
}
