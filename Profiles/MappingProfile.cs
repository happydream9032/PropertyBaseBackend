using System;
using AutoMapper;
using PropertyBase.DTOs.Property.AddProperty;
using PropertyBase.DTOs.Property.SaveDraft;
using PropertyBase.Entities;

namespace PropertyBase.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Request, Property>().ReverseMap();
            CreateMap<DraftRequest, Property>().ReverseMap();
        }
    }
}

