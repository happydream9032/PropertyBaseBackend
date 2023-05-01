using System;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PropertyBase.Contracts;
using PropertyBase.Data.Repositories;
using PropertyBase.DTOs.Property;

namespace PropertyBase.Features.Properties.GetLatestProperties
{
    public class GetLatestPropertiesHandler : IRequestHandler<GetLatestPropertiesRequest, GetLatestPropertiesResponse>
    {

        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public GetLatestPropertiesHandler(
            IPropertyRepository propertyRepository,
            IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<GetLatestPropertiesResponse> Handle(GetLatestPropertiesRequest request, CancellationToken cancellationToken)
        {
            var latestProperties = await _propertyRepository.GetQueryable()
                                              .Include(c => c.Images)
                                              .OrderByDescending(c => c.PublishedDate)
                                              .Take(request.count)
                                              .Select(c => _mapper.Map(c, new PropertyOverviewVM()))
                                              .ToListAsync();

            return new GetLatestPropertiesResponse
            {
                Properties = latestProperties
            };
        }
    }
}

