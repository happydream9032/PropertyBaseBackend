using System;
using PropertyBase.DTOs.Property;
using PropertyBase.DTOs.User;
using PropertyBase.Entities;

namespace PropertyBase.Features.Properties.GetPropertyDetails
{
    public class GetPropertyDetailsResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Locality { get; set; }
        public string Street { get; set; }
        public PropertyStatus Status { get; set; }
        public PropertyAvailability Availability { get; set; }
        public PropertyType PropertyType { get; set; }
        public double Price { get; set; }
        public PropertyPriceType PriceType { get; set; }
        public int NumberOfBedrooms { get; set; }
        public int NumberOfBathrooms { get; set; }
        public int NumberOfToilets { get; set; }
        public int? ParkingSpace { get; set; }
        public double? TotalLandArea { get; set; }
        public bool? Furnished { get; set; }
        public bool? Serviced { get; set; }
        public bool? Shared { get; set; }
        public List<PropertyImageVM> Images { get; set; }
        public DateTime PublishedDate { get; set; }
        public string? OwnerId { get; set; }
        public UserProfileVM? Owner { get; set; }
        public Guid? AgencyId { get; set; }
        public PropertyAgencyVM? Agency { get; set; }
    }
}

