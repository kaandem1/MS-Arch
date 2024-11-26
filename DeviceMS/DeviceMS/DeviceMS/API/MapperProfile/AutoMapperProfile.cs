using AutoMapper;
using DeviceMS.Core.DomainLayer.Models;
using DeviceMS.API.DTOModels;

namespace DeviceMS.API.MapperProfile
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<DeviceCreateDTO, Device>()
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.DeviceName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.MaxHourlyCons, opt => opt.MapFrom(src => src.MaxHourlyCons));

            CreateMap<Device, DeviceDTO>()
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.DeviceName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.MaxHourlyCons, opt => opt.MapFrom(src => src.MaxHourlyCons));

            CreateMap<DeviceUpdateDTO, Device>()
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.DeviceName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.MaxHourlyCons, opt => opt.MapFrom(src => src.MaxHourlyCons));

            CreateMap<PersonReferenceDTO, PersonReference>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<PersonReference, PersonReferenceDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
        }
    }
}
