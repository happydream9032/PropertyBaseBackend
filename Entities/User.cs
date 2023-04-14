using System;
using Microsoft.AspNetCore.Identity;

namespace PropertyBase.Entities
{
    public class User : IdentityUser
    {
        public User()
        {
            AllowNewPropertyNotifications = false;
            AllowRentDueNotifications = false;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime RegistrationDate { get; set; }

        public bool AllowNewPropertyNotifications { get; set; }

        public bool AllowRentDueNotifications { get; set; }

        public bool AllowRentPaymentNotifications { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Gender { get; set; }

        public string? EmploymentStatus { get; set; }

        public string? AvatarUrl { get; set; }

        public string? ImageFileId { get; set; }

        public double ProfileCompletionPercentage { get; set; }
    }
}

