using System;
using AutoMapper;
using PropertyBase.DTOs.Property;
using PropertyBase.DTOs.Property.AddProperty;
using PropertyBase.DTOs.Property.SaveDraft;
using PropertyBase.DTOs.User;
using PropertyBase.Entities;

namespace PropertyBase.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Request, Property>().ReverseMap();
            CreateMap<DraftRequest, Property>().ReverseMap();
            CreateMap<User, UserProfileVM>();
            CreateMap<PropertyImage, PropertyImageVM>();
            CreateMap<Property, PropertyOverviewVM>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));
                
        }
    }
}

