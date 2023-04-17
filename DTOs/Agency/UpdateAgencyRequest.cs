using System;

namespace PropertyBase.DTOs.Agency
{
    public class UpdateAgencyRequest
    {
        public Guid AgencyId { get; set; }
        public string? AgencyName { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Street { get; set; }
    }
}

