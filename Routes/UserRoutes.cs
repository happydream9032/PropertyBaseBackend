using System;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PropertyBase.Contracts;
using PropertyBase.DTOs.Authentication;
using PropertyBase.DTOs.User;
using PropertyBase.Entities;
using PropertyBase.Exceptions;
using AutoMapper;
using PropertyBase.DTOs.Email;
using PropertyBase.Services;

namespace PropertyBase.Routes
{
    public static class UserRoutes
    {
        public static RouteGroupBuilder UserApi(this RouteGroupBuilder group)
        {
            group.MapPost("/authenticate", async(
                [FromBody] AuthenticationRequest request,
                [FromServices] IAuthenticationService authService
                ) =>
            {
                return Results.Ok(await authService.AuthenticateAsync(request));
            });


            group.MapPost("/register", async (
                [FromBody] RegistrationRequest request,
                [FromServices] IAuthenticationService authService
                ) =>
            {
                return Results.Ok(await authService.RegisterAsync(request));
            });


            group.MapGet("/verifyEmail/{email}/{confirmationToken}", async (
                string email,
                string confirmationToken,
                HttpResponse response,
                [FromServices] UserManager<User> userManager
                ) =>
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


            group.MapPost("/forgetPassword/{email}", async (
                string email,
                [FromServices] IUserRepository userRepository) =>
            {
                return Results.Ok(await userRepository.ForgetPassword(email));
            });


            group.MapPost("/resetPassword", async (
                [FromBody] PasswordResetRequest request,
                [FromServices] IUserRepository userRepository
                ) =>
            {
                return Results.Ok(await userRepository.ResetPassword(request));
            });


            group.MapPost("/changePassword", async (
                [FromBody] PasswordUpdateRequest request,
                [FromServices] IUserRepository userRepository
                ) =>
            {
                return Results.Ok(await userRepository.UpdatePassword(request));
            }).RequireAuthorization();


            group.MapPut("/updateProfile", async (
                [FromBody] UserProfileUpdateRequest request,
                [FromServices] IUserRepository userRepository,
                [FromServices] ILoggedInUserService loggedInUserService
                ) =>
            {
                var userId = loggedInUserService.UserId;
                var user = await userRepository
                            .GetQueryable()
                            .Where(c => c.Id == userId)
                            .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, $"User with id {userId} not found");
                }

                if (!String.IsNullOrEmpty(request.Gender))
                {
                    user.Gender = request.Gender;
                }
                if (!String.IsNullOrEmpty(request.EmploymentStatus))
                {
                    user.EmploymentStatus = request.EmploymentStatus;
                }
                
                if (request.AllowNewPropertyNotifications.HasValue)
                {
                    user.AllowNewPropertyNotifications = (bool)request.AllowNewPropertyNotifications;
                }

                if (request.AllowRentDueNotifications.HasValue)
                {
                    user.AllowRentDueNotifications = (bool)request.AllowRentDueNotifications;
                }

                if (request.AllowRentPaymentNotifications.HasValue)
                {
                    user.AllowRentPaymentNotifications = (bool)request.AllowRentPaymentNotifications;
                }

                if (!String.IsNullOrEmpty(request.FirstName)) user.FirstName = request.FirstName;
                if (!String.IsNullOrEmpty(request.LastName)) user.LastName = request.LastName;
                if (!String.IsNullOrEmpty(request.Email)) user.Email = request.Email;
                if (!String.IsNullOrEmpty(request.PhoneNumber)) user.PhoneNumber = request.PhoneNumber;
                if (!String.IsNullOrEmpty(request.City)) user.City = request.City;
                if (!String.IsNullOrEmpty(request.State)) user.State = request.State;

                await userRepository.SaveChangesAsync();

                return Results.Ok(user);

            }).RequireAuthorization();

            group.MapPut("/updateProfilePhoto", async (IFormFile file,
                [FromServices] IUserRepository userRepository,
                [FromServices] IFileStorageService fileStorageService,
                [FromServices] ILoggedInUserService loggedInUserService
                ) =>
            {
                var userId = loggedInUserService.UserId;
                var user = await userRepository
                            .GetQueryable()
                            .Where(c => c.Id == userId)
                            .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, $"User with id {userId} not found");
                }

                if (!String.IsNullOrEmpty(user.ImageFileId))
                {
                    await fileStorageService.DeleteFile(user.ImageFileId);
                }

                var uploadedFile = await fileStorageService.Upload(file,ImageStorageFolder.Profile);
                user.ImageFileId = uploadedFile.fileId;
                user.AvatarUrl = uploadedFile.url;
                await userRepository.SaveChangesAsync();
                return Results.Ok(uploadedFile);

            }).RequireAuthorization();

            group.MapGet("/profile", async (
                [FromServices] IUserRepository userRepository,
                [FromServices] ILoggedInUserService loggedInUserService,
                IMapper mapper
                ) =>
            {
                var userId = loggedInUserService.UserId;
                var user = await userRepository
                            .GetQueryable()
                            .Where(c => c.Id == userId)
                            .Select(c=>mapper.Map(c,new UserProfileVM()))
                            .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, $"User with id {userId} not found");
                }

                return Results.Ok(user);

            }).RequireAuthorization();

            return group;
        }
    }
}

