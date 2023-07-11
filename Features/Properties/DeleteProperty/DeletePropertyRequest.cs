using System;
using MediatR;

namespace PropertyBase.Features.Properties.DeleteProperty
{
    public class DeletePropertyRequest : IRequest<DeletePropertyResponse>
    {
        public Guid PropertyId { get; set; }
    }
}

