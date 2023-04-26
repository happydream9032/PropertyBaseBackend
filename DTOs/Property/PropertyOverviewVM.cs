using System;
using AutoMapper;
using PropertyBase.Entities;

namespace PropertyBase.DTOs.Property
{
    public class PropertyOverviewVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int NumberOfBathrooms { get; set; }
        public int NumberOfBedrooms { get; set; }
        public double? TotalLandArea { get; set; }
        public List<PropertyImageVM> Images { get; set; }
        public double Price { get; set; }
        public PropertyPriceType PriceType { get; set; }
        public string Locality { get; set; }
        public string Street { get; set; }
    }
}

