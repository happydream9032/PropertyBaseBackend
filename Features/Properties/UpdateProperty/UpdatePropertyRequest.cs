using System;
using MediatR;
using PropertyBase.Entities;

namespace PropertyBase.Features.Properties.UpdateProperty
{
    public class UpdatePropertyRequest : IRequest<UpdatePropertyResponse>
    {
        public Guid PropertyId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Locality { get; set; }
        public string? Street { get; set; }
        public PropertyType? PropertyType { get; set; }
        public double? Price { get; set; }
        public PropertyPriceType? PriceType { get; set; }
        public int? NumberOfBedrooms { get; set; }
        public int? NumberOfBathrooms { get; set; }
        public int? NumberOfToilets { get; set; }
        public int? ParkingSpace { get; set; }
        public double? TotalLandArea { get; set; }
        public bool? Furnished { get; set; }
        public bool? Serviced { get; set; }
        public bool? Shared { get; set; }
    }
}

