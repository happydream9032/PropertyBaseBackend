using System;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyBase.Contracts;
using PropertyBase.Data.Repositories;
using PropertyBase.Entities;
using PropertyBase.Exceptions;
using PropertyBase.Services;

namespace PropertyBase.Features.Properties.UpdateProperty
{
    public class UpdatePropertyHandler : IRequestHandler<UpdatePropertyRequest,UpdatePropertyResponse>
    {

        private readonly IPropertyRepository _propertyRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUserRepository _userRepo;
        private readonly IAgencyRepository _agencyRepo;
        private readonly ILoggedInUserService _loggedInUserService;
        private readonly IMapper _mapper;

        public UpdatePropertyHandler(IPropertyRepository propertyRepository,
            IFileStorageService fileStorageService,
            IUserRepository userRepo,
            IAgencyRepository agencyRepo,
            ILoggedInUserService loggedInUserService,
            IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _fileStorageService = fileStorageService;
            _userRepo = userRepo;
            _agencyRepo = agencyRepo;
            _loggedInUserService = loggedInUserService;
            _mapper = mapper;
        }

        public async Task<UpdatePropertyResponse> Handle(UpdatePropertyRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdatePropertyValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.Errors.Count > 0)
            {
                var validationErrors = new List<string>();

                foreach (var error in validationResult.Errors)
                {
                    validationErrors.Add(error.ErrorMessage);
                }

                throw new RequestException(StatusCodes.Status400BadRequest, validationErrors.FirstOrDefault());
            }

            var property = await _propertyRepository.GetByIdAsync(request.PropertyId);

            if(property == null)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, "Property not found.");
            }

            if(property.CreatedByUserId != _loggedInUserService.UserId)
            {
                throw new RequestException(StatusCodes.Status401Unauthorized, "You are not authorized to update this property. Kindly contact the owner.");
            }


            var propertyRequestData = _mapper.Map<Property>(request);

            await _propertyRepository.UpdateAsync(propertyRequestData);
            return new UpdatePropertyResponse
            {
                Message = "Property updated successfully.",
                Success = true
            };
        }
    }
}

