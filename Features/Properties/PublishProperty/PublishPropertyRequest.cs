using System;
using MediatR;

namespace PropertyBase.Features.Properties.PublishProperty
{
    public class PublishPropertyRequest : IRequest<PublishPropertyResponse>
    {
        public Guid PropertyId { get; set; }
    }
}

