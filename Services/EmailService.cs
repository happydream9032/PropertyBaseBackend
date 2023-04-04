using System;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Net.Http.Headers;
using PropertyBase.Contracts;
using PropertyBase.DTOs.Email;
using PropertyBase.Entities;
using PropertyBase.Services.EmailTemplates;
using static System.Net.Mime.MediaTypeNames;

namespace PropertyBase.Services
{
    public class EmailService : IEmailService
    {

        private readonly IHttpClientFactory _httpClientFactory;
        
        public EmailService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string GenerateHtmlForEmailConfirmation(User user,string confirmationToken)
        {
            var emailVerificationUrl = $"{Environment.GetEnvironmentVariable("BACKEND_URL")}/api/accounts/verifyEmail/{user.Email}/{confirmationToken}";
            var frontendUrl = $"{Environment.GetEnvironmentVariable("FRONTEND_URL")}";
            return EmailConfirmationTemplate.GenerateTemplate(user, frontendUrl, emailVerificationUrl);
        }

        public async void sendMail(EmailRequest emailRequest)
        {
            if (!Configuration.Default.ApiKey.ContainsKey("api-key"))
            {
                Configuration.Default.ApiKey.Add("api-key", Environment.GetEnvironmentVariable("SENDINBLUE_API_KEY"));
            }
            
            var apiInstance = new TransactionalEmailsApi();
            //var httpClient = _httpClientFactory.CreateClient();
            var sender = new SendSmtpEmailSender(emailRequest.Sender.Name,emailRequest.Sender.Email);
            var receiver = new SendSmtpEmailTo(emailRequest.Receiver.Email, emailRequest.Receiver.Name);

            var to = new List<SendSmtpEmailTo>();
            to.Add(receiver);

            var sendSmtpEmail = new SendSmtpEmail
            {
                Sender = sender,
                To = to,
                HtmlContent = emailRequest.HTMLContent,
                Subject = emailRequest.Subject,
            };

            await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
        }

       
    }
}

