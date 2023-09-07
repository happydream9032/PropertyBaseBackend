using System;
using Microsoft.AspNetCore.Mvc;
using PropertyBase.Contracts;
using PropertyBase.DTOs.Email;
using PropertyBase.DTOs.User;

namespace PropertyBase.Routes
{
    public static class HomeRoutes
    {
        public static RouteGroupBuilder HomeApi(this RouteGroupBuilder group)
        {
            group.MapPost("/contactUs", (
                [FromBody] ContactUs request,
                [FromServices] IEmailService emailService
                ) =>
            {
                var sender = new EmailUser(request.Name, request.Email);
                var recipient = new EmailUser("Property Forager Team", Environment.GetEnvironmentVariable("SUPPORT_EMAIL")!);


                var emailBody = $"{request.Message}<br/><br/>{request.Name}<br/>{request.PhoneNumber}";
                var emailRequest = new EmailRequest(sender, recipient, "Property Forager Contact Form Submission: Inquiry", emailBody);

                emailService.sendMail(emailRequest);

                return Results.Ok(new { Message = "Message sent!" });

            });

            return group;

        }
         
    }
}

