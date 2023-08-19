using System;
using MediatR;
using PropertyBase.DTOs;
using PropertyBase.Entities;

namespace PropertyBase.Features.Properties.GetPropertiesForAgency
{
    public class GetPropertiesForAgencyRequest : PagedRequest, IRequest<GetPropertiesForAgencyResponse>
    {
        public string? SearchKeyword { get; set; }
        public PropertyType? PropertyType { get; set; }
        public double? MaximumPrice { get; set; }
        public double? MinimumPrice { get; set; }
        public PropertyPriceType? PriceType { get; set; }
        public int? NumberOfBedrooms { get; set; }
        public int? NumberOfBathrooms { get; set; }
        public int? NumberOfToilets { get; set; }
        public int? MaximumParkingSpace { get; set; }
        public int? MinimumParkingSpace { get; set; }
        public bool? Furnished { get; set; }
        public bool? Serviced { get; set; }
        public bool? Shared { get; set; }
        public string? OwnerId { get; set; }
        public Guid? AgencyId { get; set; }
        public PropertyStatus? Status { get; set; }
    }
}

