using System;
using System.Text;

namespace PropertyBase.Exceptions
{
    public class RequestStatusCodeHandler
    {
        private readonly RequestDelegate _next;
        public RequestStatusCodeHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (RequestException ex)
            {
                httpContext.Response.ContentType = "application/json";
                var response = $"{{\"Message\":\"{ex.ResponseMessage}\"}}";
                var responseData = Encoding.UTF8.GetBytes(response);

                httpContext.Response.StatusCode = (int)ex.StatusCode;
                await httpContext.Response.Body.WriteAsync(responseData);
            }
        }
    }
}

