using System;
namespace PropertyBase.DTOs.User
{
    public class PasswordUpdateRequest
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}

