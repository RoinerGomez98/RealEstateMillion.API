using AutoMapper;
using RealEstateMillion.Application.DTOs.Property;
using RealEstateMillion.Application.DTOs.PropertyImage;
using RealEstateMillion.Domain.Entities;

namespace RealEstateMillion.Application.Mappings
{
    public class PropertyMappingProfile : Profile
    {
        public PropertyMappingProfile()
        {
            CreateMap<CreatePropertyRequest, Property>();

            CreateMap<Property, PropertyResponse>()
                .ForMember(dest => dest.PropertyTypeName, opt => opt.MapFrom(src => src.PropertyType.ToString()))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ListingTypeName, opt => opt.MapFrom(src => src.ListingType.ToString()))
                .ForMember(dest => dest.ConditionName, opt => opt.MapFrom(src => src.Condition.ToString()))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.Name))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.PropertyImages))
                .ForMember(dest => dest.PrimaryImageUrl, opt => opt.MapFrom(src =>
                    src.PropertyImages.FirstOrDefault(img => img.IsPrimary && img.Enabled) != null
                        ? src.PropertyImages.FirstOrDefault(img => img.IsPrimary && img.Enabled)!.File
                        : src.PropertyImages.FirstOrDefault(img => img.Enabled)!.File));

            CreateMap<AddImageRequest, PropertyImage>();

            CreateMap<PropertyImage, PropertyImageResponse>();
        }
    }
}
