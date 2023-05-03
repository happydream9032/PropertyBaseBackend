using System;
using PropertyBase.DTOs.User;
using PropertyBase.Entities;

namespace PropertyBase.Features.Properties.GetPropertyDetails
{
    public class PropertyAgencyVM
    {
        public Guid Id { get; set; }
        public string? AgencyName { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Street { get; set; }
        public string? OwnerId { get; set; }
        public UserProfileVM? Owner { get; set; }
    }
}

