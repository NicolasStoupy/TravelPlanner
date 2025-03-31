using AutoMapper;
using BussinessLogic.DTOs;
using Infrastructure.EntityModels;

namespace BussinessLogic.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
             CreateMap<Trip, TripDTO>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToDateTime(TimeOnly.MinValue)))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ToDateTime(TimeOnly.MinValue)));

             CreateMap<TripDTO, Trip>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.StartDate)))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.EndDate)));
        }
    }
}
