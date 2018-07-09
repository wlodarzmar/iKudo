using System.Net;

namespace iKudo.Controllers.Api
{
    public class ErrorResult
    {
        public ErrorResult(string error, HttpStatusCode statusCode) : this(error, (int)statusCode)
        { }

        public ErrorResult(string error, int statusCode)
        {
            Message = error;
            StatusCode = statusCode;
        }

        public string Message { get; set; }

        public int StatusCode { get; set; }

        public string MoreInfo { get; set; }
    }
}
