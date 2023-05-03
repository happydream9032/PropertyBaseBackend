using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MediatR;
using PropertyBase.Contracts;
using PropertyBase.Data.Repositories;
using PropertyBase.Exceptions;
using PropertyBase.DTOs.User;

namespace PropertyBase.Features.Properties.GetPropertyDetails
{
    public class GetPropertyDetailsHandler : IRequestHandler<GetPropertyDetailsRequest, GetPropertyDetailsResponse>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IMapper _mapper;

        public GetPropertyDetailsHandler(
            IPropertyRepository propertyRepository,
            IAgencyRepository agencyRepository,
            IUserRepository userRepository,
            IMapper mapper
            )
        {
            _propertyRepository = propertyRepository;
            _agencyRepository = agencyRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<GetPropertyDetailsResponse> Handle(GetPropertyDetailsRequest request, CancellationToken cancellationToken)
        {
            var property =  await _propertyRepository.GetQueryable()
                                               .Include(c => c.Images)
                                               .Include(c => c.Owner)
                                               .Include(c => c.Agency)
                                               .ThenInclude(a => a.Owner)
                                               .Where(c => c.Id == request.PropertyId)
                                               .Select(c=> _mapper.Map(c,new GetPropertyDetailsResponse()))
                                               .FirstOrDefaultAsync();

            if(property == null)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, $"Property with Id {request.PropertyId} was not found.");
            }

            return property;
        }
    }
}

