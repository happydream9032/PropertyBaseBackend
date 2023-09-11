using System;
namespace PropertyBase.DTOs.Property
{
    public class PropertyInspectionRequestRejection
    {
        public PropertyInspectionRequestRejection()
        {
            RejectionReason = String.Empty;
        }

        public string RejectionReason { get; set; }
    }
}

