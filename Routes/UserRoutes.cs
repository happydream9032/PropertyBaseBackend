using System;
using Microsoft.AspNetCore.Mvc;
using PropertyBase.Contracts;
using PropertyBase.DTOs.Authentication;

namespace PropertyBase.Routes
{
    public static class UserRoutes
    {
        public static RouteGroupBuilder UserApi(this RouteGroupBuilder group)
        {
            group.MapPost("/authenticate", async([FromBody] AuthenticationRequest request,[FromServices] IAuthenticationService authService) =>
            {
                return Results.Ok(await authService.AuthenticateAsync(request));
            });
            group.MapPost("/register", async ([FromBody] RegistrationRequest request, [FromServices] IAuthenticationService authService) =>
            {
                return Results.Ok(await authService.RegisterAsync(request));
            });
            return group;
        }
    }
}

