using UserMS.API.DTOModels;
using AutoMapper;
using UserMS.Core.DomainLayer.Models;
using UserMS.Core.DomainLayer.Enums;
using AutoMapper.Execution;

namespace UserMS.API.MapperProfile
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserCreateDTO, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));

            CreateMap<User, UserDTO>()
                        .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                        .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                        .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                        .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));
            CreateMap<UserUpdateDTO, User>()
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
               .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
               .ForMember(dest => dest.Email, opt => opt.UseDestinationValue());


        }
    }
}
