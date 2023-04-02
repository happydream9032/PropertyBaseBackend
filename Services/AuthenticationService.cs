using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PropertyBase.Contracts;
using PropertyBase.DTOs.Authentication;
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

        public AuthenticationService(UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtSettings> jwtSettings,
            IAgencyRepository agencyRepository
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
            _agencyRepository = agencyRepository;
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if(user == null)
            {
                throw new RequestException(StatusCodes.Status401Unauthorized, $"User with email {request.Email}");
            }

            var signinResult = await _signInManager.PasswordSignInAsync(user.UserName, request.Password,false, lockoutOnFailure: false);
            if (!signinResult.Succeeded)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, "Invalid Credentials");
            }

            var token = await GenerateToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            return new AuthenticationResponse()
            {
                Email = request.Email,
                UserName = user.UserName,
                Id = user.Id,
                Roles = roles,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

        public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest request)
        {
           
           if(await _userManager.FindByEmailAsync(request.Email) == null)
            {
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
                
                if(request.RoleType == RoleType.Tenant)
                {
                    user.AllowNewPropertyNotifications = true;
                    user.AllowRentDueNotifications = true;
                    user.ProfileCompletionPercentage = 50.00;
                }

                if(request.RoleType == RoleType.Agency || request.RoleType == RoleType.PropertyOwner)
                {
                    user.AllowRentPaymentNotifications = true;
                    user.ProfileCompletionPercentage = 50.00;
                }
                var register = await _userManager.CreateAsync(user,request.Password);

                if (register.Succeeded)
                {
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

                    if(role == null)
                    {
                        var createRole = await _roleManager.CreateAsync(new IdentityRole(roleName));

                        if (!createRole.Succeeded)
                        {
                            throw new RequestException(StatusCodes.Status400BadRequest, "Failed to create account type");
                        }
                    }

                    if(request.RoleType == RoleType.Agency)
                    {
                        var nameSet = String.IsNullOrEmpty(request.AgencyName) ? 0.00 : 1.00;
                        var citySet = String.IsNullOrEmpty(request.AgencyCity) ? 0.00 : 1.00;
                        var stateSet = String.IsNullOrEmpty(request.AgencyState) ? 0.00 : 1.00;

                        var profileCompletionPercentage = Math.Round((nameSet + citySet + stateSet) / 3 * 100,2);
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

                    return new RegistrationResponse() { Message = "Registration successful" };

                }
                else
                {
                    throw new RequestException(StatusCodes.Status400BadRequest, register.Errors?.FirstOrDefault()?.Description);
                }
                
            }
            else
            {
                throw new RequestException(StatusCodes.Status400BadRequest, $"Email {request.Email} already exists.");
            }
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
                new Claim("uuid",user.Id)
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

