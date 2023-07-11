using System;
using MediatR;

namespace PropertyBase.Features.Properties.DeleteImage
{
    public class DeleteImageRequest : IRequest<DeleteImageResponse>
    {
        public Guid PropertyId { get; set; }
        public string FileId { get; set; }
    }
}

