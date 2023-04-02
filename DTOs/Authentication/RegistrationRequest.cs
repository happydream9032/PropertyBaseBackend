using System;
using System.ComponentModel.DataAnnotations;
using PropertyBase.Entities;

namespace PropertyBase.DTOs.Authentication
{
    public class RegistrationRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public string? AgencyCity { get; set; }
        public string? AgencyState { get; set; }
        public string? AgencyName { get; set; }

        [Required]
        public RoleType RoleType { get; set; }
    }
}

