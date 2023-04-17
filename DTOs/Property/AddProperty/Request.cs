using System;
using PropertyBase.Entities;

namespace PropertyBase.DTOs.Property.AddProperty
{
    public class Request
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string Locality { get; set; }
        public required string Street { get; set; }
        public required PropertyType PropertyType { get; set; }
        public required double Price { get; set; }
        public required PropertyPriceType PriceType { get; set; }
        public required int NumberOfBedrooms { get; set; }
        public required int NumberOfBathrooms { get; set; }
        public required int NumberOfToilets { get; set; }
        public int? ParkingSpace { get; set; }
        public double? TotalLandArea { get; set; }
        public bool? Furnished { get; set; }
        public bool? Serviced { get; set; }
        public bool? Shared { get; set; }
        public IFormFileCollection? files { get; set; }
    }
}

