using System;
using PropertyBase.DTOs.Email;
using PropertyBase.Entities;

namespace PropertyBase.Contracts
{
    public interface IEmailService
    {
        void sendMail(EmailRequest emailRequest);
        string GenerateHtmlForEmailConfirmation(User user, string confirmationToken);
        string GenerateHtmlForPasswordReset(User user, string resetToken);
    }
}

