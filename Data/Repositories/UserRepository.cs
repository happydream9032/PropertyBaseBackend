using System;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using PropertyBase.Contracts;
using PropertyBase.DTOs;
using PropertyBase.DTOs.Email;
using PropertyBase.DTOs.User;
using PropertyBase.Entities;
using PropertyBase.Exceptions;

namespace PropertyBase.Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly UserManager<User> _userManger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public UserRepository(PropertyBaseDbContext dbContext,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService
            ): base(dbContext)
        {
            _roleManager = roleManager;
            _userManger = userManager;
            _emailService = emailService;
        }

        public async Task<BaseResponse> ForgetPassword(string email)
        {
            var user = await _userManger.FindByEmailAsync(email);
            if(user == null)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, $"no user record found for {email}");
            }

            var resetToken = await _userManger.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));
            var htmlContent = _emailService.GenerateHtmlForPasswordReset(user, encodedToken);

            var sender = new EmailUser("Property Forager", Environment.GetEnvironmentVariable("SUPPORT_EMAIL")!);
            var recipient = new EmailUser($"{user.FirstName} {user.LastName}", user.Email!);

            var emailRequest = new EmailRequest(sender, recipient, "Password Reset", htmlContent);

            _emailService.sendMail(emailRequest);

            var response = new BaseResponse
            {
                Message = "Password Reset Email sent successfully",
                Success = true
            };

            return response;
        }

        public async Task<BaseResponse> ResetPassword(PasswordResetRequest request)
        {
            var user = await _userManger.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, $"no user record found for {request.Email}");
            }
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManger.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            if (!result.Succeeded)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, result.Errors?.FirstOrDefault()?.Description);
            }

            return new BaseResponse
            {
                Message = "Password has been reset successfully",
                Success = true
            };
        }

        public async Task<BaseResponse> UpdatePassword(PasswordUpdateRequest request)
        {
            var user = await _userManger.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, $"no user record found for {request.Email}");
            }
            var result = await _userManger.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, result.Errors?.FirstOrDefault()?.Description);
            }
            
            return new BaseResponse
            {
                Message = "Password has been changed successfully",
                Success = true
            };
        }

        public async Task<bool> UserHasRole(User user, RoleType roleType)
        {
            var roleName = "";
            switch (roleType)
            {
                case RoleType.Agency:
                    roleName = Role.Agency;
                    break;
                case RoleType.PropertyOwner:
                    roleName = Role.PropertyOwner;
                    break;
                default:
                    roleName = Role.Tenant;
                    break;
            }
            var role = await _roleManager.FindByNameAsync(roleName);
            return await _userManger.IsInRoleAsync(user,role?.Name!);
        }
    }
}

