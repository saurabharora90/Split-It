using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Split_It_.Utils
{
    class Constants
    {
        public static String DATABASE_NAME = "splitwise.sqlite";
        public static string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, DATABASE_NAME);

        public static String SPLITWISE_API_URL = " https://secure.splitwise.com/api/v3.0/";
        public static String SPLITWISE_AUTHORIZE_URL = "https://secure.splitwise.com/authorize";
        public static String OAUTH_CALLBACK = "http://saurabhsplit";
        public static String consumerKey = "wQukgHsUKubX5gal5rVKFXJ48lfvhxTXL9H12SkE";
        public static String consumerSecret = "tt9wZlnI3MKOsvW3bKlrFflCEM5MvKtrOV4Ba8U8";

        public static String ACCESS_TOKEN_TAG = "access_token";
        public static String ACCESS_TOKEN_SECRET_TAG = "access_token_secret";

        public static String LAST_UPDATED_TIME = "last_update";
        public static String CURRENT_USER_ID = "current_user_id";

        public static string SELECTED_USER = "selected_user";
        public static string SELECTED_EXPENSE = "selected_expense";

        public static string CRITTERCISM_ID = "5360ad740729df4d95000002";
    }
}
