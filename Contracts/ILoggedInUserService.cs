using System;
namespace PropertyBase.Contracts
{
    public interface ILoggedInUserService
    {
        public string UserId { get; }
        public string UserEmail { get; }
    }
}

