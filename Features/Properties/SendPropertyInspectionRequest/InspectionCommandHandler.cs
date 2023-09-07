using System;
using Microsoft.EntityFrameworkCore;
using MediatR;
using PropertyBase.Contracts;
using PropertyBase.Entities;
using PropertyBase.Exceptions;
using PropertyBase.DTOs.Email;

namespace PropertyBase.Features.Properties.SendPropertyInspectionRequest
{
    public class InspectionCommandHandler : IRequestHandler<InspectionCommand,InspectionResponse>
    {
        private readonly IPropertyInspectionRequestRepository _propertyInspectionRequestRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILoggedInUserService _loggedInUserService;

        public InspectionCommandHandler(
            IPropertyInspectionRequestRepository propertyInspectionRequestRepository,
            IPropertyRepository propertyRepository,
            IAgencyRepository agencyRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            ILoggedInUserService loggedInUserService
            )
        {
            _propertyInspectionRequestRepository = propertyInspectionRequestRepository;
            _propertyRepository = propertyRepository;
            _agencyRepository = agencyRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _loggedInUserService = loggedInUserService;
        }

        public async  Task<InspectionResponse> Handle(InspectionCommand request, CancellationToken cancellationToken)
        {
            if(!request.PropertyAgencyId.HasValue && String.IsNullOrEmpty(request.PropertyOwnerId))
            {
                throw new RequestException(StatusCodes.Status400BadRequest, "You must specify the id of the agency or of the owner related to the property.");
            }

            var validator = new InspectionCommandValidator();
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

            var existingRequest = await _propertyInspectionRequestRepository.GetQueryable()
                                         .Where(c => c.CreatedByUserId == _loggedInUserService.UserId && c.PropertyId == request.PropertyId)
                                         .FirstOrDefaultAsync();

            if(existingRequest != null)
            {
                throw new RequestException(StatusCodes.Status400BadRequest,"You have already requested to inspect this property. Kindly check your dashbaord for the status of the request.");
            }

            var property = await _propertyRepository.GetByIdAsync(request.PropertyId);

            if(property == null)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, $"Property with id {request.PropertyId} does not exist.");
            };

            var propertyInspectionRequest = new PropertyInspectionRequest
            {
                SenderFullName = request.SenderFullName,
                SenderEmail = request.SenderEmail,
                PropertyId = request.PropertyId,
                PhoneNumber = request.PhoneNumber,
                Stage = PropertyInspectionStage.Pending
            };

            if (request.PropertyAgencyId.HasValue)
            {
                var agency = await _agencyRepository.GetQueryable()
                                                    .Include(c=>c.Owner)
                                                    .Where(c => c.Id == request.PropertyAgencyId)
                                                    .FirstOrDefaultAsync();

                if(agency == null)
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, $"Agency with id {request.PropertyAgencyId} does not exist.");
                }

                if(_loggedInUserService.UserId == agency.OwnerId)
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, "You cannot request to inspect your own property.");
                }

                propertyInspectionRequest.PropertyAgencyId = agency.Id;

                await _propertyInspectionRequestRepository.AddAsync(propertyInspectionRequest);

                var sender = new EmailUser("Property Forager", Environment.GetEnvironmentVariable("SUPPORT_EMAIL")!);
                var recipient = new EmailUser($"{agency.AgencyName}", agency.Owner.Email!);
                var emailHtmlContent = _emailService.GenerateHtmlForPropertyInspectionEmail(agency.AgencyName ?? "Agent", request.SenderFullName, request.SenderEmail, property.Id);
                var emailRequest = new EmailRequest(sender, recipient, $"Property Inspection for {property.Street}, {property.Locality}", emailHtmlContent);

                _emailService.sendMail(emailRequest);

                return new InspectionResponse
                {
                    Message = "Property Inspection Request sent successfully.",
                    Success = true
                };
            }
            else
            {
                var propertyOwner = await _userRepository.GetQueryable()
                                        .Where(c => c.Id == request.PropertyOwnerId)
                                        .FirstOrDefaultAsync();

                if(propertyOwner == null)
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, $"Property Owner with id {request.PropertyOwnerId} does not exist.");
                }

                if (_loggedInUserService.UserId == propertyOwner.Id)
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, "You cannot request to inspect your own property.");
                }

                propertyInspectionRequest.PropertyOwnerId = propertyOwner.Id;

                await _propertyInspectionRequestRepository.AddAsync(propertyInspectionRequest);

                var sender = new EmailUser("Property Forager", Environment.GetEnvironmentVariable("SUPPORT_EMAIL")!);
                var recipient = new EmailUser($"{propertyOwner.FirstName} {propertyOwner.LastName}", propertyOwner.Email!);
                var emailHtmlContent = _emailService.GenerateHtmlForPropertyInspectionEmail($"{propertyOwner.FirstName} {propertyOwner.LastName}", request.SenderFullName, request.SenderEmail, property.Id);
                var emailRequest = new EmailRequest(sender, recipient, $"Property Inspection for {property.Street}, {property.Locality}", emailHtmlContent);

                _emailService.sendMail(emailRequest);

                return new InspectionResponse
                {
                    Message = "Property Inspection Request sent successfully.",
                    Success = true
                };

            }

        }
    }
}

