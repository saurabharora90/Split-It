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

        public static void setLastUpdatedTime()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            int unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            settings.Add(Constants.LAST_UPDATED_TIME, unixTimestamp);
            settings.Save();
        }

        public static int getLastUpdatedTime()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.LAST_UPDATED_TIME))
            {
                return Convert.ToInt32(IsolatedStorageSettings.ApplicationSettings[Constants.LAST_UPDATED_TIME]);
            }
            return 0;
        }

        public static void setCurrentUserId(int userId)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Add(Constants.CURRENT_USER_ID, userId);
            settings.Save();
        }

        public static int getCurrentUserId()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.CURRENT_USER_ID))
            {
                return Convert.ToInt32(IsolatedStorageSettings.ApplicationSettings[Constants.CURRENT_USER_ID]);
            }
            return 0;
        }
    }
}
