using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyBase.Contracts;
using PropertyBase.DTOs.Property.AddProperty;
using PropertyBase.DTOs.Property.SaveDraft;
using PropertyBase.Entities;
using PropertyBase.Exceptions;

namespace PropertyBase.Routes
{
    public static class PropertyRoutes
    {
       public static RouteGroupBuilder PropertyApi(this RouteGroupBuilder group)
        {
            group.MapPost("/add", async ([FromBody] Request request,
                [FromServices] IPropertyRepository propertyRepository,
                [FromServices] IFileStorageService fileStorageService,
                 IMapper _mapper,
                 IHttpContextAccessor contextAccessor
                ) =>
            {
                var loggedInUserId = contextAccessor.HttpContext?.User?.FindFirst("uid")?.Value;

                var validator = new Validator();
                var validationResult = await validator.ValidateAsync(request);

                if(validationResult.Errors.Count > 0)
                {
                    var validationErrors = new List<string>();

                    foreach (var error in validationResult.Errors)
                    {
                        validationErrors.Add(error.ErrorMessage);
                    }

                    throw new RequestException(StatusCodes.Status400BadRequest, validationErrors.FirstOrDefault());
                }

                List<PropertyImage> propertyImages = new();

                if (request.files is not null && request.files.Count > 0)
                {
                    
                    foreach (var file in request.files)
                    {
                        var uploadedFile = await fileStorageService.Upload(file, ImageStorageFolder.Property);

                        propertyImages.Add(new PropertyImage
                        {
                            ImageURL = uploadedFile.url,
                            Verified = true
                        });
                        
                    }
                }

                var property = _mapper.Map<Property>(request);

                property.Images = propertyImages;
                property.Availability = PropertyAvailability.Available;
                property.Status = PropertyStatus.Published;
                property.PublishedDate = DateTime.UtcNow;

                await propertyRepository.AddAsync(property);
                return Results.Ok(new Response
                {
                    Message = "Property added successfully.",
                    Success = true
                });
            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

            group.MapPost("/saveDraft", async ([FromBody] DraftRequest request,
                [FromServices] IPropertyRepository propertyRepository,
                [FromServices] IFileStorageService fileStorageService,
                 IMapper _mapper,
                 IHttpContextAccessor contextAccessor
                ) =>
            {
                var loggedInUserId = contextAccessor.HttpContext?.User?.FindFirst("uid")?.Value;

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

                List<PropertyImage> propertyImages = new();

                if (request.files is not null && request.files.Count > 0)
                {

                    foreach (var file in request.files)
                    {
                        var uploadedFile = await fileStorageService.Upload(file, ImageStorageFolder.Property);

                        propertyImages.Add(new PropertyImage
                        {
                            ImageURL = uploadedFile.url,
                            Verified = true
                        });

                    }
                }

                var property = _mapper.Map<Property>(request);

                property.Images = propertyImages;
                property.Availability = PropertyAvailability.Available;
                property.Status = PropertyStatus.Draft;

                await propertyRepository.AddAsync(property);
                return Results.Ok(new Response
                {
                    Message = "Property successfully saved as draft.",
                    Success = true
                });
            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

            group.MapPost("{propertyId}/publish", async (Guid propertyId,
                [FromServices] IPropertyRepository propertyRepository,
                 IMapper _mapper,
                 IHttpContextAccessor contextAccessor
                ) =>
            {
                var loggedInUserId = contextAccessor.HttpContext?.User?.FindFirst("uid")?.Value;

                var property = await propertyRepository.GetByIdAsync(propertyId);

                if(property == null)
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, "Property does not exist.");
                }

                if(property.CreatedByUserId != loggedInUserId)
                {
                    throw new RequestException(StatusCodes.Status401Unauthorized, "You are not authorized to publish a Property you did not create.");
                }

                property.Status = PropertyStatus.Published;
                property.PublishedDate = DateTime.UtcNow;

                await propertyRepository.AddAsync(property);
                return Results.Ok(new Response
                {
                    Message = "Property published successfully.",
                    Success = true
                });
            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

            return group;
        }
    }
}

