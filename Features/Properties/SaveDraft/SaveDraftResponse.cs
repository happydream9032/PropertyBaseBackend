using System;
using PropertyBase.DTOs;

namespace PropertyBase.Features.Properties.SaveDraft
{
    public class SaveDraftResponse : BaseResponse
    {
        public SaveDraftResponse() : base()
        {
        }

        public Guid PropertyId { get; set; }
    }
}

