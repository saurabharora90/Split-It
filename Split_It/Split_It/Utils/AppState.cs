using Split_It.Model;

namespace Split_It.Utils
{
    public class AppState
    {
        public static string AccessToken { get; set; }
        public static string AccessTokenSecret { get; set; }
        public static User CurrentUser { get; set; }
    }
}
