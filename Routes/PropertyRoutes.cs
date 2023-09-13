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
using PropertyBase.Features.Properties.GetPropertiesForAgency;
using PropertyBase.Features.Properties.SendPropertyInspectionRequest;
using PropertyBase.DTOs.Email;

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

            group.MapPost("/inspection/request", async (
                [FromBody] InspectionCommand command,
                IMediator _mediator
                ) =>
            {
                return Results.Ok(await _mediator.Send(command));

            }).RequireAuthorization(AuthorizationPolicy.TenantPolicy);

            group.MapGet("/inspection/request", async (
               [FromQuery(Name="agencyId")] Guid? agencyId,
               [FromServices] IPropertyInspectionRequestRepository propertyInspectionRequestRepository,
               [FromServices] ILoggedInUserService loggedInUserService
               ) =>
            {
                var propertyInspectionRequests = propertyInspectionRequestRepository.GetQueryable()
                                                       .OrderByDescending(c => c.CreatedAt)
                                                       .AsNoTracking();
                if (agencyId.HasValue)
                {
                    propertyInspectionRequests = propertyInspectionRequests.Where(c => c.PropertyAgencyId == agencyId);
                }
                else
                {
                    propertyInspectionRequests = propertyInspectionRequests
                                                        .Where(c => c.CreatedByUserId == loggedInUserService.UserId ||
                                                               c.SenderEmail == loggedInUserService.UserEmail ||
                                                               c.PropertyOwnerId == loggedInUserService.UserId
                                                               );
                                                       
                                              
                }
                var propertyInspectionRequestsData = await propertyInspectionRequests.ToListAsync();

                return Results.Ok(new {InspectionRequestData= propertyInspectionRequestsData });
                                                        
            }).RequireAuthorization();

            group.MapPut("/inspection/request/{requestId}/accept", async (
                Guid requestId,
               [FromServices] IPropertyInspectionRequestRepository propertyInspectionRequestRepository,
               [FromServices] IAgencyRepository agencyRepository,
               [FromServices] IUserRepository userRepository,
               [FromServices] ILoggedInUserService loggedInUserService,
                [FromServices] IEmailService emailService
               ) =>
            {
                var propertyInspectionRequestQuery = propertyInspectionRequestRepository.GetQueryable()
                                                       .Where(c => c.Id == requestId);

                var user = await userRepository.GetQueryable()
                                  .Where(c => c.Id == loggedInUserService.UserId)
                                  .FirstOrDefaultAsync();

                if (await userRepository.UserHasRole(user, RoleType.Agency))
                {
                    var agency = await agencyRepository.GetQueryable()
                                 .Include(c => c.Owner)
                                 .Where(c => c.OwnerId == loggedInUserService.UserId)
                                 .FirstOrDefaultAsync();
                    propertyInspectionRequestQuery = propertyInspectionRequestQuery
                                                          .Where(c => c.PropertyAgencyId == agency.Id);
                }
                else
                {
                    propertyInspectionRequestQuery = propertyInspectionRequestQuery
                                                          .Where(c => c.PropertyOwnerId == user.Id);
                }

                var propertyInspectionRequest = await propertyInspectionRequestQuery.FirstOrDefaultAsync();
                if (propertyInspectionRequest == null)
                {
                    return Results.BadRequest(new { Message = "Resource not found." });
                }

                if (propertyInspectionRequest.Stage == PropertyInspectionStage.InProgress)
                {
                    return Results.BadRequest(new { Message = "Property inspection request has been accepted already." });
                }

                propertyInspectionRequest.Stage = PropertyInspectionStage.InProgress;

                await propertyInspectionRequestRepository.SaveChangesAsync();

                var sender = new EmailUser("Property Forager", Environment.GetEnvironmentVariable("SUPPORT_EMAIL")!);
                var recipient = new EmailUser(propertyInspectionRequest.SenderFullName, propertyInspectionRequest.SenderEmail);

                var propertyUrl = $"{Environment.GetEnvironmentVariable("FRONTEND_URL")}/property/{propertyInspectionRequest.PropertyId}";
                var emailBody = $"Hi {propertyInspectionRequest.SenderFullName}<br/>Your request to inspect this " +
                $"<a href={propertyUrl}>property</a> has been accepted.<br/>the property agent or owner will reach out to you soon.";
                var emailRequest = new EmailRequest(sender, recipient, "Property Inspection Request Accepted", emailBody);

                emailService.sendMail(emailRequest);

                return Results.Ok(new
                {
                    Message = "Request accepted successfully",
                    Data = propertyInspectionRequest
                });

            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

            group.MapPut("/inspection/request/{requestId}/reject", async (
                Guid requestId,
               [FromBody] PropertyInspectionRequestRejection payload,
               [FromServices] IPropertyInspectionRequestRepository propertyInspectionRequestRepository,
               [FromServices] IAgencyRepository agencyRepository,
               [FromServices] IUserRepository userRepository,
               [FromServices] ILoggedInUserService loggedInUserService,
                [FromServices] IEmailService emailService
               ) =>
            {
                var propertyInspectionRequestQuery = propertyInspectionRequestRepository.GetQueryable()
                                                       .Where(c => c.Id == requestId);

                var user = await userRepository.GetQueryable()
                                  .Where(c => c.Id == loggedInUserService.UserId)
                                  .FirstOrDefaultAsync();

                if (await userRepository.UserHasRole(user, RoleType.Agency))
                {
                    var agency = await agencyRepository.GetQueryable()
                                 .Include(c => c.Owner)
                                 .Where(c => c.OwnerId == loggedInUserService.UserId)
                                 .FirstOrDefaultAsync();
                    propertyInspectionRequestQuery = propertyInspectionRequestQuery
                                                          .Where(c => c.PropertyAgencyId == agency.Id);
                }
                else
                {
                    propertyInspectionRequestQuery = propertyInspectionRequestQuery
                                                          .Where(c => c.PropertyOwnerId == user.Id);
                }

                var propertyInspectionRequest = await propertyInspectionRequestQuery.FirstOrDefaultAsync();
                if (propertyInspectionRequest == null)
                {
                    return Results.BadRequest(new { Message = "Resource not found." });
                }

                if (propertyInspectionRequest.Stage == PropertyInspectionStage.Rejected)
                {
                    return Results.BadRequest(new { Message = "Property inspection request has been rejected already." });
                }

                propertyInspectionRequest.Stage = PropertyInspectionStage.Rejected;

                await propertyInspectionRequestRepository.SaveChangesAsync();

                var sender = new EmailUser("Property Forager", Environment.GetEnvironmentVariable("SUPPORT_EMAIL")!);
                var recipient = new EmailUser(propertyInspectionRequest.SenderFullName, propertyInspectionRequest.SenderEmail);

                var propertyUrl = $"{Environment.GetEnvironmentVariable("FRONTEND_URL")}/property/{propertyInspectionRequest.PropertyId}";
                var emailBody = $"Hi {propertyInspectionRequest.SenderFullName}<br/>We are sorry to inform you that your request to inspect this " +
                $"<a href={propertyUrl}>property</a> was rejected for the following reason(s):<br/>{payload.RejectionReason}";
                var emailRequest = new EmailRequest(sender, recipient, "Property Inspection Request Rejected.", emailBody);

                emailService.sendMail(emailRequest);

                return Results.Ok(new
                {
                    Message = "Request rejected successfully",
                    Data = propertyInspectionRequest
                });

                

            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

            group.MapDelete("/inspection/request/{requestId}/cancel", async (
                Guid requestId,
               [FromServices] IPropertyInspectionRequestRepository propertyInspectionRequestRepository,
               [FromServices] IAgencyRepository agencyRepository,
               [FromServices] IUserRepository userRepository,
               [FromServices] ILoggedInUserService loggedInUserService,
                [FromServices] IEmailService emailService
               ) =>
            {
                    var propertyInspectionRequest = await propertyInspectionRequestRepository.GetQueryable()
                                                       .Where(c => c.Id == requestId &&
                                                       c.CreatedByUserId == loggedInUserService.UserId)
                                                       .FirstOrDefaultAsync();

                    if(propertyInspectionRequest == null)
                    {
                        return Results.BadRequest(new { Message = "Resource not found." });
                    }

                    await propertyInspectionRequestRepository.DeleteAsync(propertyInspectionRequest);

                    var sender = new EmailUser("Property Forager", Environment.GetEnvironmentVariable("SUPPORT_EMAIL")!);

                    if (!String.IsNullOrEmpty(propertyInspectionRequest.PropertyOwnerId))
                    {
                        var propertyOwner = await userRepository.GetQueryable()
                                                  .Where(c => c.Id == propertyInspectionRequest.PropertyOwnerId)
                                                  .FirstOrDefaultAsync();
                        if(propertyOwner != null)
                        {
                            var recipient = new EmailUser(propertyOwner.FirstName, propertyOwner.Email!);

                            var propertyUrl = $"{Environment.GetEnvironmentVariable("FRONTEND_URL")}/property/{propertyInspectionRequest.PropertyId}";
                            var emailBody = $"Hi {propertyOwner.FirstName}<br/>The request made by {loggedInUserService.UserEmail} to inspect this " +
                            $"<a href={propertyUrl}>property</a> has been cancelled.";
                            var emailRequest = new EmailRequest(sender, recipient, "Property Inspection Request Cancelled.", emailBody);

                            emailService.sendMail(emailRequest);
                        }
                        
                    }else if (propertyInspectionRequest.PropertyAgencyId.HasValue)
                    {
                        var agency = await agencyRepository.GetQueryable()
                                .Include(c => c.Owner)
                                .Where(c => c.Id == propertyInspectionRequest.PropertyAgencyId)
                                .FirstOrDefaultAsync();
                    
                        if (agency != null)
                        {
                            var recipient = new EmailUser(agency.AgencyName??"Agency", agency.Owner.Email!);

                            var propertyUrl = $"{Environment.GetEnvironmentVariable("FRONTEND_URL")}/property/{propertyInspectionRequest.PropertyId}";
                            var emailBody = $"Hi {agency.AgencyName??"Agent"}<br/>The request made by {loggedInUserService.UserEmail} to inspect this " +
                            $"<a href={propertyUrl}>property</a> has been cancelled.";
                            var emailRequest = new EmailRequest(sender, recipient, "Property Inspection Request Cancelled.", emailBody);

                            emailService.sendMail(emailRequest);
                        }
                    
                }

                    return Results.Ok(new
                    {
                        Message = "Request cancelled successfully",
                        RequestId = requestId
                    });

            }).RequireAuthorization(AuthorizationPolicy.TenantPolicy);


            group.MapPut("/inspection/request/{requestId}/complete", async (
                Guid requestId,
                [FromQuery(Name = "agencyId")] Guid? agencyId,
               [FromServices] IPropertyInspectionRequestRepository propertyInspectionRequestRepository,
               [FromServices] IUserRepository userRepository,
               [FromServices] ILoggedInUserService loggedInUserService,
                [FromServices] IEmailService emailService
               ) =>
            {
                var propertyInspectionRequest = await propertyInspectionRequestRepository.GetQueryable()
                                                       .Where(c => c.Id == requestId &&
                                                       (c.CreatedByUserId == loggedInUserService.UserId ||
                                                       c.PropertyOwnerId==loggedInUserService.UserId ||
                                                       c.PropertyAgencyId == agencyId
                                                       )
                                                       )
                                                       .FirstOrDefaultAsync();

                if (propertyInspectionRequest == null)
                {

                    return Results.BadRequest(new { Message = "Resource not found." });
                }

                if (propertyInspectionRequest.Stage == PropertyInspectionStage.Done)
                {
                    return Results.BadRequest(new { Message = "Inspection request has been completed already." });
                }

                propertyInspectionRequest.Stage = PropertyInspectionStage.Done;
                await propertyInspectionRequestRepository.SaveChangesAsync();

                return Results.Ok(new
                {
                    Message = "Inspection completed successfully",
                    RequestId = requestId
                });


            }).RequireAuthorization();



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

            group.MapPost("/propertiesForAgency", async (
               [FromBody] GetPropertiesForAgencyRequest request,
                IMediator _mediator
               ) =>
            {
                return Results.Ok(await _mediator.Send(request));
            }).RequireAuthorization(AuthorizationPolicy.PropertyPolicy);

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

