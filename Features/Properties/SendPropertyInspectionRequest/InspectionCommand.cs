using System;
using MediatR;

namespace PropertyBase.Features.Properties.SendPropertyInspectionRequest
{
    public class InspectionCommand : IRequest<InspectionResponse>
    {
        public InspectionCommand()
        {
            SenderFullName = String.Empty;
            SenderEmail = String.Empty;
        }
        public Guid PropertyId { get; set; }
        public string SenderFullName { get; set; }
        public string SenderEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? PropertyAgencyId { get; set; }
        public string? PropertyOwnerId { get; set; }
    }
}

