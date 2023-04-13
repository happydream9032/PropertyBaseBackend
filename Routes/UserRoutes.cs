using System;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PropertyBase.Contracts;
using PropertyBase.DTOs.Authentication;
using PropertyBase.DTOs.User;
using PropertyBase.Entities;
using PropertyBase.Exceptions;

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
            group.MapGet("/verifyEmail/{email}/{confirmationToken}", async (string email, string confirmationToken,HttpResponse response, [FromServices] UserManager<User> userManager) =>
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    throw new RequestException(StatusCodes.Status404NotFound, $"User with email {email} not found");
                }
                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmationToken));
                
                var results = await userManager.ConfirmEmailAsync(user, decodedToken);
                
                if(!results.Succeeded) throw new RequestException(StatusCodes.Status400BadRequest, results.Errors.FirstOrDefault()?.Description);
                
                response.Redirect($"{Environment.GetEnvironmentVariable("FRONTEND_URL")!}/login");
                
            });

            group.MapPost("/forgetPassword/{email}", async (string email, [FromServices] IUserRepository userRepository) =>
            {
                return Results.Ok(await userRepository.ForgetPassword(email));
            });

            group.MapPost("/resetPassword", async ([FromBody] PasswordResetRequest request, [FromServices] IUserRepository userRepository) =>
            {
                return Results.Ok(await userRepository.ResetPassword(request));
            });

            group.MapPost("/changePassword", async ([FromBody] PasswordUpdateRequest request, [FromServices] IUserRepository userRepository) =>
            {
                return Results.Ok(await userRepository.UpdatePassword(request));
            }).RequireAuthorization();

            return group;
        }
    }
}

