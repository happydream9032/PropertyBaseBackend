using System;
using PropertyBase.Entities;

namespace PropertyBase.DTOs.Property
{
    public class PropertyImageVM
    {
        public Guid Id { get; set; }
        public string ImageURL { get; set; }
        public bool Verified { get; set; }
    }
}

