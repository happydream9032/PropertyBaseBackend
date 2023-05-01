using System;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PropertyBase.Contracts;
using PropertyBase.Exceptions;
using PropertyBase.Services;
using PropertyBase.Data.Repositories;
using PropertyBase.Entities;

namespace PropertyBase.Features.Properties.SaveDraft
{
    public class SaveDraftHandler : IRequestHandler<SaveDraftRequest, SaveDraftResponse>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUserRepository _userRepo;
        private readonly IAgencyRepository _agencyRepo;
        private readonly IMapper _mapper;
        private readonly ILoggedInUserService _loggedInUserService;

        public SaveDraftHandler(
            IPropertyRepository propertyRepository,
            IFileStorageService fileStorageService,
            IUserRepository userRepo,
            IAgencyRepository agencyRepo,
            IMapper mapper,
            ILoggedInUserService loggedInUserService
            )
        {
            _propertyRepository = propertyRepository;
            _fileStorageService = fileStorageService;
            _userRepo = userRepo;
            _agencyRepo = agencyRepo;
            _mapper = mapper;
            _loggedInUserService = loggedInUserService;
        }

        public async Task<SaveDraftResponse> Handle(SaveDraftRequest request, CancellationToken cancellationToken)
        {
            var validator = new SaveDraftValidator();
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

            if (String.IsNullOrEmpty(user!.AvatarUrl))
            {
                throw new RequestException(StatusCodes.Status403Forbidden, "Please upload your profile photo and try again.");
            }

            var property = _mapper.Map<Property>(request);

            property.Availability = PropertyAvailability.Available;
            property.Status = PropertyStatus.Draft;

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

            return new SaveDraftResponse
            {
                Message = "Property successfully saved as draft.",
                Success = true
            };
        }
    }
}

