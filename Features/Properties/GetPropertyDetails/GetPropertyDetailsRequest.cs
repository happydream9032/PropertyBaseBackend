using System;
using MediatR;

namespace PropertyBase.Features.Properties.GetPropertyDetails
{
    public class GetPropertyDetailsRequest : IRequest<GetPropertyDetailsResponse>
    {
        public Guid PropertyId { get; set; }
    }
}

