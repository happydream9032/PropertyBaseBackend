using System;
namespace PropertyBase.Exceptions
{
    public class RequestException : Exception
    {
        public RequestException(int statusCode, string? responseMessage)
        {
            StatusCode = statusCode;
            ResponseMessage = responseMessage;
        }

        public int StatusCode { get; set; }
        public string? ResponseMessage { get; set; }
    }
}

