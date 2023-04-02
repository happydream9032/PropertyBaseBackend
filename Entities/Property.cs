using System;
namespace PropertyBase.Entities
{
    public class Property : BaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Locality { get; set; }
        public string Street { get; set; }
        public PropertyStatus Status { get; set; }
        public PropertyAvailability Availability { get; set; }
        public PropertyType Type { get; set; }
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
        public List<PropertyImage> Images { get; set; }
        public Guid? AgencyId { get; set; }
        public Agency? Agency { get; set; }
    }
}

