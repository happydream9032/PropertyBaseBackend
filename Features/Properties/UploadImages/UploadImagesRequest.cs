using System;
using MediatR;

namespace PropertyBase.Features.Properties.UploadImages
{
    public class UploadImagesRequest : IRequest<UploadImagesResponse>
    {
        public UploadImagesRequest()
        {
            Files = new FormFileCollection();
        }
        public Guid PropertyId { get; set; }
        public IFormFileCollection Files { get; set; }
    }
}

