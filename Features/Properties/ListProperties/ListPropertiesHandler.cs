using System;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyBase.Contracts;
using PropertyBase.DTOs.Property;

namespace PropertyBase.Features.Properties.ListProperties
{
    public class ListPropertiesHandler : IRequestHandler<ListPropertiesRequest,ListPropertiesResponse>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public ListPropertiesHandler(
            IPropertyRepository propertyRepository,
            IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<ListPropertiesResponse> Handle(ListPropertiesRequest request, CancellationToken cancellationToken)
        {
            var propertiesQueryable = _propertyRepository.GetQueryable();

            if(request.AgencyId.HasValue && String.IsNullOrEmpty(request.OwnerId))
            {
                propertiesQueryable = propertiesQueryable.Where(c => c.AgencyId == request.AgencyId);
            }

            if (!String.IsNullOrEmpty(request.OwnerId) && !request.AgencyId.HasValue)
            {
                propertiesQueryable = propertiesQueryable.Where(c => c.OwnerId == request.OwnerId);
            }

            if (request.Furnished.HasValue)
            {
                propertiesQueryable = propertiesQueryable.Where(c => c.Furnished.HasValue && c.Furnished.Value);
            }

            if (!String.IsNullOrEmpty(request.Locality))
            {
                propertiesQueryable = propertiesQueryable.Where(c =>
                !String.IsNullOrEmpty(c.Locality) && c.Locality.Contains(request.Locality));
            }

            if (request.MaximumParkingSpace.HasValue)
            {
                propertiesQueryable = propertiesQueryable.Where(c =>
                c.ParkingSpace.HasValue && c.ParkingSpace <= request.MaximumParkingSpace);
            }

            if (request.MinimumParkingSpace.HasValue)
            {
                propertiesQueryable = propertiesQueryable.Where(c =>
                c.ParkingSpace.HasValue && c.ParkingSpace>=request.MinimumParkingSpace);
            }

            if (request.MaximumPrice.HasValue && request.PriceType.HasValue)
            {
                propertiesQueryable = propertiesQueryable.Where(c =>
                c.Price <= request.MaximumPrice && c.PriceType == request.PriceType);
            }

            if (request.MinimumPrice.HasValue && request.PriceType.HasValue)
            {
                propertiesQueryable = propertiesQueryable.Where(c =>
                c.Price >= request.MinimumPrice && c.PriceType == request.PriceType);
            }

            if (request.PropertyType.HasValue)
            {
                propertiesQueryable = propertiesQueryable.Where(c => c.PropertyType == request.PropertyType);
            }

            if (request.Serviced.HasValue)
            {
                propertiesQueryable = propertiesQueryable.Where(c => c.Serviced.HasValue && c.Serviced.Value);
            }

            if (request.Shared.HasValue)
            {
                propertiesQueryable = propertiesQueryable.Where(c => c.Shared.HasValue && c.Shared.Value);
            }

            if (!String.IsNullOrEmpty(request.Street))
            {
                propertiesQueryable = propertiesQueryable.Where(c =>
                !String.IsNullOrEmpty(c.Street) && c.Street.Contains(request.Street));
            }

            if (request.NumberOfBathrooms.HasValue)
            {
                propertiesQueryable = propertiesQueryable.Where(c => c.NumberOfBathrooms == request.NumberOfBathrooms);
            }

            if (request.NumberOfBedrooms.HasValue)
            {
                propertiesQueryable = propertiesQueryable.Where(c => c.NumberOfBedrooms == request.NumberOfBedrooms);
            }

            if (request.NumberOfToilets.HasValue)
            {
                propertiesQueryable = propertiesQueryable.Where(c => c.NumberOfToilets == request.NumberOfToilets);
            }

            var propertieslist = propertiesQueryable
                                      .Skip((request.PageNumber - 1) * request.PageSize)
                                      .Take(request.PageSize)
                                      .Include(c => c.Images)
                                      .OrderByDescending(c => c.PublishedDate);


            return new ListPropertiesResponse
            {
                Data = await propertieslist
                           .Select(c => _mapper.Map(c, new PropertyOverviewVM()))
                           .ToListAsync(),
                Count = await propertiesQueryable.CountAsync()
            };

        }
    }
}

