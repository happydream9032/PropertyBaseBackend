using System;
namespace PropertyBase.Entities
{
    public class Agency : BaseEntity
    {
        public Guid Id { get; set; }
        public string? AgencyName { get; set; }
        public List<Property> Properties { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string OwnerId { get; set; }
        public User Owner { get; set; }
        public double ProfileCompletionPercentage { get; set; }
    }
}

