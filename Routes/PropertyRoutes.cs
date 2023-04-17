using System;
using Microsoft.AspNetCore.Mvc;
using PropertyBase.Contracts;
using PropertyBase.DTOs.Property.AddProperty;
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

                if(request.files is not null && request.files.Count > 0)
                {
                    List<PropertyImage> propertyImages = new();

                    foreach (var file in request.files)
                    {
                        var uploadedFile = await fileStorageService.Upload(file, ImageStorageFolder.Property);
                        
                    }
                }

            });

            return group;
        }
    }
}

