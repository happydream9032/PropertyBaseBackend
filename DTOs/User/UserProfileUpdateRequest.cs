using System;
using PropertyBase.Entities;

namespace PropertyBase.DTOs.User
{
    public class UserProfileUpdateRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Gender { get; set; }
        public string? EmploymentStatus { get; set; }
        public bool? AllowNewPropertyNotifications { get; set; }
        public bool? AllowRentDueNotifications { get; set; }
        public bool? AllowRentPaymentNotifications { get; set; }
    }
}

