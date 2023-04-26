using System;
using System.Security.Claims;
using PropertyBase.Contracts;

namespace PropertyBase.Services
{
    public class LoggedInUserService : ILoggedInUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public LoggedInUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string UserId => _contextAccessor.HttpContext.User?.FindFirst("uid")?.Value;

        public string UserEmail => _contextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
}

