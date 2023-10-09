using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDTO>()
        .ForMember(dest => dest.PhotoURL, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
        //This is custom mapping configuration for specific memebers of the source and destinaton type. Here it maps the MemberDTO variable photourl to the url returned from AppUser where the photo isMain is true i.e they have a profile pic

        //sql querying efficeincy reasons we calc the age of users here
        .ForMember(dest => dest.Age, opt => opt.MapFrom
        (src => src.DateOfBirth.CalculateAge()));
        
        CreateMap<Photo,PhotoDTO>();

        CreateMap<MemberUpdateDTO,AppUser>();
        
        CreateMap<RegisterDto,AppUser>();

    
    }
}
