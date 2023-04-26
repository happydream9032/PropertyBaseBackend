using System;
using PropertyBase.DTOs;
using PropertyBase.DTOs.User;
using PropertyBase.Entities;

namespace PropertyBase.Contracts
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<BaseResponse> ForgetPassword(string email);
        Task<BaseResponse> ResetPassword(PasswordResetRequest request);
        Task<BaseResponse> UpdatePassword(PasswordUpdateRequest request);
        Task<bool> UserHasRole(User user, RoleType roleType);
    }
}

