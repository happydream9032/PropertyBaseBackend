using System;
using PropertyBase.DTOs.Property;

namespace PropertyBase.Features.Properties.GetLatestProperties
{
    public class GetLatestPropertiesResponse
    {
        public GetLatestPropertiesResponse()
        {
            Properties = new List<PropertyOverviewVM>();
        }
        public List<PropertyOverviewVM> Properties { get; set; }
    }
}

