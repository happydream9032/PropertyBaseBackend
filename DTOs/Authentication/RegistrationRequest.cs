using System;
using System.ComponentModel.DataAnnotations;
using PropertyBase.Entities;

namespace PropertyBase.DTOs.Authentication
{
    public class RegistrationRequest
    {
        
        public required string FirstName { get; set; }
        
        public required string LastName { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [Phone]
        public required string PhoneNumber { get; set; }

        [MinLength(6)]
        public required string Password { get; set; }

        public string? AgencyCity { get; set; }
        public string? AgencyState { get; set; }
        public string? AgencyName { get; set; }

        public required RoleType RoleType { get; set; }
    }
}

