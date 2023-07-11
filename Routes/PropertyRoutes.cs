using System;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyBase.Contracts;
using PropertyBase.Entities;
using PropertyBase.Exceptions;
using FluentValidation;
using PropertyBase.Services;
using PropertyBase.DTOs;
using PropertyBase.DTOs.Property;
using PropertyBase.DTOs.User;
using PropertyBase.Features.Properties.UpdateProperty;
using MediatR;
using PropertyBase.Features.Properties.UploadImages;
using PropertyBase.Features.Properties.SaveDraft;
using PropertyBase.Features.Properties.PublishProperty;
using PropertyBase.Features.Properties.GetLatestProperties;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PropertyBase.Features.Properties.ListProperties;
using PropertyBase.Features.Properties.GetPropertyDetails;
using Newtonsoft.Json;
using PropertyBase.Features.Properties.DeleteImage;
using PropertyBase.Features.Properties.DeleteProperty;

namespace PropertyBase.Routes
{
    public static class PropertyRoutes
    {
       public static RouteGroupBuilder PropertyApi(this RouteGroupBuilder group)
        {
            group.MapPost("/saveDraft", async (
               [FromBody] SaveDraftRequest request,
                IMediator _mediator
               ) =>
            {
                return Results.Ok(await _mediator.Send(request));
            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

            group.MapPut("/update", async (
                [FromBody] UpdatePropertyRequest request,
                IMediator _mediator
                ) =>
            {
                return Results.Ok(await _mediator.Send(request));
                
            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

            group.MapDelete("/{propertyId}/delete", async (
                Guid propertyId,
                IMediator _mediator
                ) =>
            {
                return Results.Ok(await _mediator.Send(new DeletePropertyRequest { PropertyId=propertyId}));

            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

            group.MapPost("/{propertyId}/uploadImages", async (
                Guid propertyId,
                IMediator _mediator,
                IFormFileCollection files
                ) =>
            {
                return Results.Ok(await _mediator.Send(
                    new UploadImagesRequest { PropertyId=propertyId,Files=files})
                    );
            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

            group.MapPost("/deleteImage", async (
                [FromBody] DeleteImageRequest request,
                IMediator _mediator
                ) =>
            {
                return Results.Ok(await _mediator.Send(request));
            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

            group.MapPost("/{propertyId}/publish", async (
                Guid propertyId,
                IMediator _mediator
                ) =>
            {
                return Results.Ok(await _mediator.Send(
                    new PublishPropertyRequest { PropertyId=propertyId})
                    );
            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

            group.MapPost("/latestProperties", async (
                [FromBody] GetLatestPropertiesRequest request,
                 IMediator _mediator
                ) =>
            {
                return Results.Ok(await _mediator.Send(request));
            });

            group.MapGet("/{propertyId}", async (
                Guid propertyId,
                IMediator _mediator
                ) =>
            {
                return Results.Ok(await _mediator.Send(
                    new GetPropertyDetailsRequest { PropertyId=propertyId})
                    );
            });

            group.MapPost("/list", async (
                [FromBody] ListPropertiesRequest request,
                 IMediator _mediator
                ) =>
            {
                return Results.Ok(await _mediator.Send(request));
            });

            group.MapGet("/searchLocations", async (String searchKeyWord) =>
            {
                var locationsApiUrl = $"{Environment.GetEnvironmentVariable("LOCACTIONS_API_BASE_URL")}?keywords={searchKeyWord}&type=localities-sub-localities-only&dataType=json";
                var httpClient = new HttpClient();

                var response = await (await httpClient.GetAsync(locationsApiUrl)).Content.ReadAsStringAsync();
                return response;
            });

            return group;
        }
    }
}

