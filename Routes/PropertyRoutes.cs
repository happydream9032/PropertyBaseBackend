using System;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyBase.Contracts;
using PropertyBase.DTOs.Property.AddProperty;
using PropertyBase.DTOs.Property.SaveDraft;
using PropertyBase.Entities;
using PropertyBase.Exceptions;
using FluentValidation;
using PropertyBase.Services;
using PropertyBase.DTOs;
using PropertyBase.DTOs.Property;
using PropertyBase.DTOs.User;

namespace PropertyBase.Routes
{
    public static class PropertyRoutes
    {
       public static RouteGroupBuilder PropertyApi(this RouteGroupBuilder group)
        {
            group.MapPost("/add", async (
                [FromBody] Request request,
                [FromServices] IPropertyRepository propertyRepository,
                [FromServices] IFileStorageService fileStorageService,
                [FromServices] IUserRepository userRepo,
                [FromServices] IAgencyRepository agencyRepo,
                [FromServices] ILoggedInUserService loggedInUserService,
                 IMapper _mapper
                ) =>
            {
                var loggedInUserId = loggedInUserService.UserId;

                var user = await userRepo.GetQueryable()
                                 .Where(c => c.Id == loggedInUserId)
                                 .FirstOrDefaultAsync();

                if (String.IsNullOrEmpty(user?.AvatarUrl))
                {
                    throw new RequestException(StatusCodes.Status403Forbidden, "Please upload your profile photo and try again.");
                }

                var validator = new Validator();
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

                var property = _mapper.Map<Property>(request);

                property.Availability = PropertyAvailability.Available;
                property.Status = PropertyStatus.Published;
                property.PublishedDate = DateTime.UtcNow;

                if((await userRepo.UserHasRole(user, RoleType.Agency)))
                {
                    property.AgencyId = agencyRepo.GetQueryable()
                                             .Where(c => c.OwnerId == user.Id)
                                             .FirstOrDefaultAsync()
                                             .GetAwaiter()
                                             .GetResult()?.Id;
                }
                else
                {
                    property.OwnerId = user.Id;
                }

                await propertyRepository.AddAsync(property);
                return Results.Ok(new Response
                {
                    Message = "Property added successfully. The next step is to upoad the property's images.",
                    Success = true
                });
            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

            group.MapPost("{propertyId}/uploadImages", async (
                Guid propertyId,
                [FromServices] IPropertyRepository propertyRepository,
                [FromServices] IFileStorageService fileStorageService,
                IFormFileCollection files
                ) =>
            {
                if (files.Count == 0)
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, "No image file was sent in the request.");
                }

                foreach (var file in files)
                {
                    if (!fileStorageService.ValidateFileSize(file))
                    {
                        throw new RequestException(StatusCodes.Status400BadRequest, "Each file's size must not exceed 10MB.");
                    }
                }

                var property = await propertyRepository.GetByIdAsync(propertyId);

                if (property == null)
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, "Property does not exist.");
                }

                List<PropertyImage> propertyImages = new();

                foreach (var file in files)
                {
                    var uploadedFile = await fileStorageService.Upload(file, ImageStorageFolder.Property);

                    propertyImages.Add(new PropertyImage
                    {
                        ImageURL = uploadedFile.url,
                        Verified = true
                    });

                }

                property.Images = propertyImages;

                await propertyRepository.SaveChangesAsync();

                return Results.Ok(new BaseResponse
                {
                    Message = "Images uploaded successfully",
                    Success = true
                });
            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);
            

            group.MapPost("/saveDraft", async (
                [FromBody] DraftRequest request,
                [FromServices] IPropertyRepository propertyRepository,
                [FromServices] IFileStorageService fileStorageService,
                 [FromServices] IUserRepository userRepo,
                  [FromServices] IAgencyRepository agencyRepo,
                 IMapper _mapper,
                 [FromServices] ILoggedInUserService loggedInUserService
                ) =>
            {
                var loggedInUserId = loggedInUserService.UserId;
                var user = await userRepo.GetQueryable()
                                 .Where(c => c.Id == loggedInUserId)
                                 .FirstOrDefaultAsync();

                if (String.IsNullOrEmpty(user!.AvatarUrl))
                {
                    throw new RequestException(StatusCodes.Status403Forbidden, "Please upload your profile photo and try again.");
                }
                var validator = new DraftValidator();
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

                var property = _mapper.Map<Property>(request);

                property.Availability = PropertyAvailability.Available;
                property.Status = PropertyStatus.Draft;

                if ((await userRepo.UserHasRole(user, RoleType.Agency)))
                {
                    property.AgencyId = agencyRepo.GetQueryable()
                                             .Where(c => c.OwnerId == user.Id)
                                             .FirstOrDefaultAsync()
                                             .GetAwaiter()
                                             .GetResult()?.Id;
                }
                else
                {
                    property.OwnerId = user.Id;
                }

                await propertyRepository.AddAsync(property);

                return Results.Ok(new Response
                {
                    Message = "Property successfully saved as draft.",
                    Success = true
                });
            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

            group.MapPost("{propertyId}/publish", async (
                Guid propertyId,
                [FromServices] IPropertyRepository propertyRepository,
                 IMapper mapper,
                 [FromServices] ILoggedInUserService loggedInUserService
                ) =>
            {
                var loggedInUserId = loggedInUserService.UserId;

                var property = await propertyRepository.GetByIdAsync(propertyId);

                if(property == null)
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, "Property does not exist.");
                }

                if(property.Status == PropertyStatus.Published)
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, "You cannot publish a Property that has been published already.");
                }

                var existingPropertyData = mapper.Map(property, new Request());
                var validator = new Validator();
                var validationResult = await validator.ValidateAsync(existingPropertyData);

                if (validationResult.Errors.Count > 0)
                {

                    throw new RequestException(StatusCodes.Status400BadRequest, "Property cannot be published because it is missing some required information. Please update it and try again.");
                }

                if (property.CreatedByUserId != loggedInUserId)
                {
                    throw new RequestException(StatusCodes.Status401Unauthorized, "You are not authorized to publish a Property you did not create.");
                }

                property.Status = PropertyStatus.Published;
                property.PublishedDate = DateTime.UtcNow;

                await propertyRepository.SaveChangesAsync();
                return Results.Ok(new Response
                {
                    Message = "Property published successfully.",
                    Success = true
                });
            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

            group.MapGet("/latestProperties", async (
                [FromServices] IPropertyRepository propertyRepository,
                IMapper mapper
                ) =>
            {
                return await propertyRepository.GetQueryable()
                                              .Include(c => c.Images)
                                              .OrderByDescending(c => c.PublishedDate)
                                              .Take(4)
                                              .Select(c=>mapper.Map(c,new PropertyOverviewVM()))
                                              .ToListAsync();
            });

            group.MapGet("{propertyId}/details", async (
                Guid propertyId,
                [FromServices] IPropertyRepository propertyRepository,
                IMapper mapper
                ) =>
            {
                return await propertyRepository.GetQueryable()
                                              .Include(c => c.Images)
                                              .Include(c =>c.Owner)
                                              .Include(c => c.Agency)
                                              .ThenInclude(a=>a.Owner)
                                              .Where(c => c.Id == propertyId)
                                              .FirstOrDefaultAsync();
            });

            return group;
        }
    }
}

