using System.Net;

namespace Split_It.Events
{
    public class ApiErrorEvent
    {
        public HttpStatusCode StatusCode { get; private set; }

        public ApiErrorEvent(HttpStatusCode code)
        {
            StatusCode = code;
        }
    }
}
