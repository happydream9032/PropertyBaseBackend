using System;
using MediatR;

namespace PropertyBase.Features.Properties.GetLatestProperties
{
    public class GetLatestPropertiesRequest : IRequest<GetLatestPropertiesResponse>
    {
        public GetLatestPropertiesRequest()
        {
            count = 4;
        }
        public int count { get; set; }
    }
}

