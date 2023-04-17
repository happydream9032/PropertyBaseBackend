using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PropertyBase.Contracts;
using PropertyBase.DTOs.Agency;
using PropertyBase.Exceptions;

namespace PropertyBase.Routes
{
    public static class AgencyRoutes
    {
        public static RouteGroupBuilder AgencyApi(this RouteGroupBuilder group)
        {
            group.MapPut("/update", async ([FromBody] UpdateAgencyRequest request,
                [FromServices] IAgencyRepository agencyRepository,
                 IHttpContextAccessor contextAccessor
                ) =>
            {
                var userId = contextAccessor.HttpContext?.User?.FindFirst("uid")?.Value;
                var agency = await agencyRepository.GetQueryable()
                                  .Where(c => c.Id == request.AgencyId && c.OwnerId == userId)
                                  .FirstOrDefaultAsync();
                if(agency == null)
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, $"No Agency found for user with id {userId}");
                }

                if (!String.IsNullOrEmpty(request.AgencyName)) agency.AgencyName = request.AgencyName;
                if (!String.IsNullOrEmpty(request.City)) agency.City = request.City;
                if (!String.IsNullOrEmpty(request.State)) agency.State = request.State;
                if (!String.IsNullOrEmpty(request.Street))
                {
                    agency.Street = request.Street;
                    if(agency.ProfileCompletionPercentage != 100)
                    {
                        agency.ProfileCompletionPercentage = 100;
                    }
                }

                await agencyRepository.UpdateAsync(agency);
                return Results.Ok(new UpdateAgencyResponse
                {
                    Message = "Agency updated successfully",
                    Success = true
                });
            });

            group.MapGet("/", async ([FromServices] IAgencyRepository agencyRepository,
                 IHttpContextAccessor contextAccessor) =>
            {
                var userId = contextAccessor.HttpContext?.User?.FindFirst("uid")?.Value;
                var agency = await agencyRepository.GetQueryable()
                                             .Where(c => c.OwnerId == userId)
                                             .FirstOrDefaultAsync();
                return Results.Ok(agency);
            });
            return group;
        }
    }
}

