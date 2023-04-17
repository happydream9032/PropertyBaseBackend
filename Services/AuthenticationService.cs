using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PropertyBase.Contracts;
using PropertyBase.DTOs.Authentication;
using PropertyBase.DTOs.Email;
using PropertyBase.Entities;
using PropertyBase.Exceptions;

namespace PropertyBase.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IEmailService _emailService;

        public AuthenticationService(UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtSettings> jwtSettings,
            IAgencyRepository agencyRepository,
            IEmailService emailService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
            _agencyRepository = agencyRepository;
            _emailService = emailService;
        }
        
        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            
            if (user == null)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, $"User with email {request.Email} not found.");
            }
            if (!user.EmailConfirmed)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, $"Email {request.Email} has not been activated. " +
                    $"Please check your email and confirm it using the confirmation link sent or request for a new link");
            }
            var signinResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: false);
            
            if (signinResult.IsLockedOut)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, "Maximum signin attempts exceeded.Please wait for 5 minutes and try again");
            }

            if (!signinResult.Succeeded)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, "Invalid Credentials");
            }

            var token = await GenerateToken(user);

            var roles = await _userManager.GetRolesAsync(user);
            return new AuthenticationResponse()
            {
                Email = request.Email,
                UserName = user.UserName!,
                Id = user.Id,
                Roles = roles,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

        public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest request)
        {
            if ((await _userManager.FindByEmailAsync(request.Email)) != null)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, $"Email {request.Email} already exists.");
            }

            var user = new User
            {
                Email = request.Email,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                RegistrationDate = DateTime.UtcNow
            };

            if (request.RoleType == RoleType.Tenant)
            {
                user.AllowNewPropertyNotifications = true;
                user.AllowRentDueNotifications = true;
                user.ProfileCompletionPercentage = 50.00;
            }

            if (request.RoleType == RoleType.Agency || request.RoleType == RoleType.PropertyOwner)
            {
                user.AllowRentPaymentNotifications = true;
                user.ProfileCompletionPercentage = 50.00;
            }
            var register = await _userManager.CreateAsync(user, request.Password);

            if (!register.Succeeded)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, register.Errors?.FirstOrDefault()?.Description);
            }

            var roleName = "";
            switch (request.RoleType)
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

            if (role == null)
            {
                var createRole = await _roleManager.CreateAsync(new IdentityRole(roleName));

                if (!createRole.Succeeded)
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, "Failed to create account type");
                }
            }

            if (request.RoleType == RoleType.Agency)
            {
                var nameSet = String.IsNullOrEmpty(request.AgencyName) ? 0.00 : 1.00;
                var citySet = String.IsNullOrEmpty(request.AgencyCity) ? 0.00 : 1.00;
                var stateSet = String.IsNullOrEmpty(request.AgencyState) ? 0.00 : 1.00;

                var profileCompletionPercentage = Math.Round((nameSet + citySet + stateSet) / 4 * 100, 2);
                var agency = new Agency
                {
                    City = request.AgencyCity,
                    State = request.AgencyState,
                    AgencyName = request.AgencyName,

                    ProfileCompletionPercentage = profileCompletionPercentage,

                    CreatedByUserId = user.Id,
                    OwnerId = user.Id
                };

                await _agencyRepository.AddAsync(agency);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);

            if (!roleResult.Succeeded)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, roleResult.Errors?.FirstOrDefault()?.Description);
            }

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));
            var emailHtmlContent = _emailService.GenerateHtmlForEmailConfirmation(user, encodedToken);

            var sender = new EmailUser("Property Forager", Environment.GetEnvironmentVariable("SUPPORT_EMAIL")!);
            var recipient = new EmailUser($"{user.FirstName} {user.LastName}", user.Email);

            var emailRequest = new EmailRequest(sender, recipient, "Email Confirmation", emailHtmlContent);

            _emailService.sendMail(emailRequest);


            return new RegistrationResponse() { Message = "Registration successful" };


        }

        private async Task<JwtSecurityToken> GenerateToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < userRoles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", userRoles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid",user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signinCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signinCredentials
                );
        }
    }
}

