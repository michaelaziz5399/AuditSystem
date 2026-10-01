using AuditServer.DataBase.Dtos.AuthDtos;
using AuditServer.DataBase.Entities;
using AutoMapper;

namespace AuditServer.Mapper
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            // 1. من RegisterAuditorDto إلى Entity (نقوم بتطبيق الـ Regions والـ Roles برمجياً في الـ Service)
            CreateMap<RegisterAuditorDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.UserRegions, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

            // 2. من ApplicationUser إلى UserListDto (للقوائم الخفيفة)
            CreateMap<ApplicationUser, UserListDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty));

            // 3. من ApplicationUser إلى UserDetailsDto (للتفاصيل والـ Auth Response)
            CreateMap<ApplicationUser, UserDetailsDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty))
                .ForMember(dest => dest.RegionIds, opt => opt.MapFrom(src => src.UserRegions.Select(ur => ur.RegionId).ToList()))
                .ForMember(dest => dest.RegionNames, opt => opt.MapFrom(src => src.UserRegions.Select(ur => ur.Region.Name).ToList()))
                .ForMember(dest => dest.Roles, opt => opt.Ignore()); // تعبئة الـ Roles برمجياً بـ UserManager
        }
    }
}
