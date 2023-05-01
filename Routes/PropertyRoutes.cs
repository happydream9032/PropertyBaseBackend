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
using PropertyBase.Features.Properties.AddProperty;
using MediatR;
using PropertyBase.Features.Properties.UploadImages;
using PropertyBase.Features.Properties.SaveDraft;
using PropertyBase.Features.Properties.PublishProperty;
using PropertyBase.Features.Properties.GetLatestProperties;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PropertyBase.Features.Properties.ListProperties;
using PropertyBase.Features.Properties.GetPropertyDetails;

namespace PropertyBase.Routes
{
    public static class PropertyRoutes
    {
       public static RouteGroupBuilder PropertyApi(this RouteGroupBuilder group)
        {
            group.MapPost("/add", async (
                [FromBody] AddPropertyRequest request,
                IMediator _mediator
                ) =>
            {
                return Results.Ok(await _mediator.Send(request));
                
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
            

            group.MapPost("/saveDraft", async (
                [FromBody] SaveDraftRequest request,
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

            group.MapGet("/latestProperties", async (
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

            group.MapGet("/list", async (
                [FromBody] ListPropertiesRequest request,
                 IMediator _mediator
                ) =>
            {
                return Results.Ok(await _mediator.Send(request));
            });

            return group;
        }
    }
}

