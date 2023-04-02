using System;
namespace PropertyBase.DTOs.Authentication
{
    public class AuthenticationResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public IList<string> Roles { get; set; }
    }
}

