using System;
using PropertyBase.DTOs.Authentication;

namespace PropertyBase.Contracts
{
    public interface IAuthenticationService
    {
        Task<RegistrationResponse> RegisterAsync(RegistrationRequest registrationRequest);
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest authenticationRequest);
    }
}

