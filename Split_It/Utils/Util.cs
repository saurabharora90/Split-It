using Microsoft.Phone.Net.NetworkInformation;
using Split_It_.Model;
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
            DateTime now = DateTime.UtcNow;
            string lastUpdatedTime = now.ToString("yyyy-MM-ddTHH:mm:ssK");

            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.LAST_UPDATED_TIME))
            {
                settings.Remove(Constants.LAST_UPDATED_TIME);
            }

            settings.Add(Constants.LAST_UPDATED_TIME, lastUpdatedTime);
            settings.Save();
        }

        public static string getLastUpdatedTime()
        {
            //Last update time has to be returned in this format: 2014-04-22T12:35:16Z
            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.LAST_UPDATED_TIME))
            {
                return IsolatedStorageSettings.ApplicationSettings[Constants.LAST_UPDATED_TIME].ToString();
            }
            return "0";
        }

        public static void setCurrentUserId(int userId)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            
            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.CURRENT_USER_ID))
            {
                settings.Remove(Constants.CURRENT_USER_ID);
            }

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

        public static bool checkNetworkConnection()
        {
            var ni = NetworkInterface.NetworkInterfaceType;

            bool IsConnected = false;
            if ((ni == NetworkInterfaceType.Wireless80211) || (ni == NetworkInterfaceType.MobileBroadbandCdma) || (ni == NetworkInterfaceType.MobileBroadbandGsm))
                IsConnected = true;
            else if (ni == NetworkInterfaceType.None)
                IsConnected = false;
            return IsConnected;
        }

        public static double getBalance(List<Balance_User> balance)
        {
            double finalBalance = 0;

            if (balance == null)
                return finalBalance;

            foreach (var userBalance in balance)
            {

                finalBalance += System.Convert.ToDouble(userBalance.amount);
            }

            return finalBalance;
        }

        public static void logout()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.CURRENT_USER_ID))
            {
                settings.Remove(Constants.CURRENT_USER_ID);
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.LAST_UPDATED_TIME))
            {
                settings.Remove(Constants.LAST_UPDATED_TIME);
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.ACCESS_TOKEN_TAG))
            {
                settings.Remove(Constants.ACCESS_TOKEN_TAG);
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.ACCESS_TOKEN_SECRET_TAG))
            {
                settings.Remove(Constants.ACCESS_TOKEN_SECRET_TAG);
            }
        }
    }
}
