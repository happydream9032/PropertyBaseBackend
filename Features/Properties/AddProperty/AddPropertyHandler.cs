using System;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyBase.Contracts;
using PropertyBase.Data.Repositories;
using PropertyBase.Entities;
using PropertyBase.Exceptions;
using PropertyBase.Services;

namespace PropertyBase.Features.Properties.AddProperty
{
    public class AddPropertyHandler : IRequestHandler<AddPropertyRequest,AddPropertyResponse>
    {

        private readonly IPropertyRepository _propertyRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUserRepository _userRepo;
        private readonly IAgencyRepository _agencyRepo;
        private readonly ILoggedInUserService _loggedInUserService;
        private readonly IMapper _mapper;

        public AddPropertyHandler(IPropertyRepository propertyRepository,
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

        public async Task<AddPropertyResponse> Handle(AddPropertyRequest request, CancellationToken cancellationToken)
        {
            var validator = new AddPropertyValidator();
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

            var loggedInUserId = _loggedInUserService.UserId;

            var user = await _userRepo.GetQueryable()
                             .Where(c => c.Id == loggedInUserId)
                             .FirstOrDefaultAsync();

            if (String.IsNullOrEmpty(user?.AvatarUrl))
            {
                throw new RequestException(StatusCodes.Status403Forbidden, "Please upload your profile photo and try again.");
            }

            var property = _mapper.Map<Property>(request);

            property.Availability = PropertyAvailability.Available;
            property.Status = PropertyStatus.Published;
            property.PublishedDate = DateTime.UtcNow;

            if ((await _userRepo.UserHasRole(user, RoleType.Agency)))
            {
                property.AgencyId = _agencyRepo.GetQueryable()
                                         .Where(c => c.OwnerId == user.Id)
                                         .FirstOrDefaultAsync()
                                         .GetAwaiter()
                                         .GetResult()?.Id;
            }
            else
            {
                property.OwnerId = user.Id;
            }

            await _propertyRepository.AddAsync(property);
            return new AddPropertyResponse
            {
                Message = "Property added successfully. The next step is to upoad the property's images.",
                Success = true
            };
        }
    }
}

