using AutoMapper;
using NationalPark_API.DTOs;
using NationalPark_API.Models;

namespace NationalPark_API.DTOMapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile() 
        { 
            CreateMap<TrailDTO,Trail>().ReverseMap();
            CreateMap<NationalPark, NationalParkDTO>().ReverseMap();
        }
    }
}
//SERVER-----DB----MODEL---REPOSITORY-----DTO----CLIENT
//CLIENT----DTO-----REPOSITORY-----MODEL---DB-----SERVER
