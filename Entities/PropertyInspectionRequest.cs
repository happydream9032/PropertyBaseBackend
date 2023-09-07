using System;
namespace PropertyBase.Entities
{
    public class PropertyInspectionRequest: BaseEntity
    {
        public PropertyInspectionRequest()
        {
            SenderFullName = String.Empty;
            SenderEmail = String.Empty;
        }
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public string SenderFullName { get; set; }
        public string SenderEmail { get; set; }
        public PropertyInspectionStage Stage { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? PropertyAgencyId { get; set; }
        public string? PropertyOwnerId { get; set; }
    }
}

