using System;
namespace PropertyBase.Entities
{
    public class PropertyImage : BaseEntity
    {
        public Guid Id { get; set; }
        public string ImageURL { get; set; }
        public bool Verified { get; set; }
        public Guid PropertyId { get; set; }
        public Property Property { get; set; }
    }
}

