using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Utils
{
    class Util
    {
        public static void setAccessToken(String accessToken)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Add(Constants.ACCESS_TOKEN_TAG, accessToken);
            settings.Save();
        }

        public static void setAccessTokenSecret(String accessTokenSecret)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Add(Constants.ACCESS_TOKEN_SECRET_TAG, accessTokenSecret);
            settings.Save();
        }

        public static String getAccessToken()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.ACCESS_TOKEN_TAG))
            {
                return IsolatedStorageSettings.ApplicationSettings[Constants.ACCESS_TOKEN_TAG] as string;
            }
            return null;
        }

        public static String getAccessTokenSecret()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.ACCESS_TOKEN_SECRET_TAG))
            {
                return IsolatedStorageSettings.ApplicationSettings[Constants.ACCESS_TOKEN_SECRET_TAG] as string;
            }
            return null;
        }

        public static string GetQueryParameter(string input, string parameterName)
        {
            foreach (string item in input.Split('&'))
            {
                var parts = item.Split('=');
                if (parts[0] == parameterName)
                {
                    return parts[1];
                }
            }
            return String.Empty;
        }
    }
}
