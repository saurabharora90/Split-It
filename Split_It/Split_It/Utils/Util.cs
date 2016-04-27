using Newtonsoft.Json;
using Split_It.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Utils
{
    public class Util
    {
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

        public static object getSetting(string key)
        {
            Windows.Storage.ApplicationDataContainer localSettings =
    Windows.Storage.ApplicationData.Current.LocalSettings;
            return localSettings.Values[key];
        }

        public static void saveSetting(string key, object value)
        {
            Windows.Storage.ApplicationDataContainer localSettings =
    Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[key] = value;
        }

        public static void saveCurrentUserData(User user)
        {
            string output = JsonConvert.SerializeObject(user);
            saveSetting(Constants.SETTINGS_CURRENT_USER, output);
        }

        public static User getCurrentUserData()
        {
            string input = getSetting(Constants.SETTINGS_CURRENT_USER) as string;
            if (input == null)
                return null;
            return JsonConvert.DeserializeObject<User>(input); ;
        }
    }
}
