using System;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FluentValidation;
using MediatR;
using PropertyBase.Contracts;
using PropertyBase.Data.Repositories;
using PropertyBase.Entities;
using PropertyBase.Exceptions;
using PropertyBase.Features.Properties.SaveDraft;
using PropertyBase.Services;

namespace PropertyBase.Features.Properties.PublishProperty
{
    public class PublishPropertyHandler : IRequestHandler<PublishPropertyRequest, PublishPropertyResponse>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly ILoggedInUserService _loggedInUserService;
        private readonly IMapper _mapper;

        public PublishPropertyHandler(
            IPropertyRepository propertyRepository,
            ILoggedInUserService loggedInUserService,
            IMapper mapper
            )
        {
            _propertyRepository = propertyRepository;
            _loggedInUserService = loggedInUserService;
            _mapper = mapper;
        }

        public async Task<PublishPropertyResponse> Handle(PublishPropertyRequest request, CancellationToken cancellationToken)
        {
            var validator = new PublishPropertyValidator();
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

            var property = await _propertyRepository.GetQueryable()
                                 .Where(c => c.Id == request.PropertyId)
                                 .Include(c => c.Images)
                                 .FirstOrDefaultAsync();

            if (property == null)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, "Property does not exist.");
            }

            if (property.Status == PropertyStatus.Published)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, "You cannot publish a Property that has been published already.");
            }

            if (property.Images.Count == 0)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, "You cannot publish a property that doesn't have images. Upload images and try again.");
            }

            property.Status = PropertyStatus.Published;
            property.PublishedDate = DateTime.UtcNow;

            await _propertyRepository.SaveChangesAsync();

            return new PublishPropertyResponse
            {
                Message = "Property published successfully.",
                Success = true
            };
        }
    }
}

