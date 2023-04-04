using System;
namespace PropertyBase.DTOs.Email
{
    public record EmailRequest(
        EmailUser Sender,
        EmailUser Receiver,
        string Subject,
        string HTMLContent);
}

