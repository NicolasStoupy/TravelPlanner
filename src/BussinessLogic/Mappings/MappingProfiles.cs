using AutoMapper;
using BussinessLogic.Entities;
using BussinessLogic.Mappings.Converters;
using BussinessLogic.Mappings.Resolvers;
using Infrastructure.EntityModels;


namespace BussinessLogic.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Enregistrement des converters globaux
            CreateMap<DateOnly, DateTime>().ConvertUsing<DateOnlyToDateTimeConverter>();
            CreateMap<DateTime, DateOnly>().ConvertUsing<DateTimeToDateOnlyConverter>();



            CreateMap<Trip, Travel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TripId))
                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.people, opt => opt.MapFrom(src => src.NumberPeople))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.budget, opt => opt.MapFrom(src => src.Budget))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.travelDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.currencie, opt => opt.MapFrom(src => src.CurrencyCode))
                .ForMember(dest => dest.imageID, opt => opt.MapFrom(src => src.TripBackgroundGuid))
                .ForMember(dest => dest.image, opt => opt.MapFrom<TravelImageResolver>())
                .ForMember(dest => dest.TravelNotes, opt => opt.MapFrom<TravelNotesResolver>())
                .ForMember(dest => dest.TravelActivities, opt => opt.MapFrom<TravelActivitiesResolver>());

            ;
            CreateMap<Travel, Trip>()
                .ForMember(dest => dest.TripId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
                 .ForMember(dest => dest.NumberPeople, opt => opt.MapFrom(src => src.people))
                .ForMember(dest => dest.Budget, opt => opt.MapFrom(src => src.budget))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.TripBackgroundGuid, opt => opt.MapFrom(src => src.imageID))
                .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.currencie));




            CreateMap<TravelActivity, Activity>()
            .ForMember(dest => dest.ActivityId, opt => opt.MapFrom(src => src.ActivityID))
            .ForMember(dest => dest.ActivityTypeId, opt => opt.MapFrom(src => src.ActivityType.ID))
            .ForMember(dest => dest.ActivityDate, opt => opt.MapFrom(src => src.ActivityDate))
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
           .ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Sequence))
           .ForMember(dest => dest.GoogleLink, opt => opt.MapFrom(src => src.GoogleLink))
           .ForMember(dest => dest.PlannedCost, opt => opt.MapFrom(src => src.PlannedCost))
           .ForMember(dest => dest.TripId, opt => opt.MapFrom(src => src.TravelID))
           ;


            CreateMap<Activity, TravelActivity>()
            .ForMember(dest => dest.ActivityID, opt => opt.MapFrom(src => src.ActivityId))
            .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => src.ActivityType))
            .ForMember(dest => dest.ActivityTypeName, opt => opt.MapFrom(src => src.ActivityType.Description))
            .ForMember(dest => dest.ActivityDate, opt => opt.MapFrom(src => src.ActivityDate))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Sequence))
            .ForMember(dest => dest.GoogleLink, opt => opt.MapFrom(src => src.GoogleLink))
            .ForMember(dest => dest.PlannedCost, opt => opt.MapFrom(src => src.PlannedCost))
            .ForMember(dest => dest.Followers, opt => opt.MapFrom<TravelActivityFollowerResolver>())
            .ForMember(dest => dest.Cost, opt => opt.MapFrom<TravelActivityCostResolver>())
            .ForMember(dest => dest.Notes, opt => opt.MapFrom<TravelActivityNotesResolver>());

            CreateMap<LogBook, Note>()
             .ForMember(dest => dest.NoteId, opt => opt.MapFrom(src => src.LogBookId))
             .ForMember(dest => dest.NoteContent, opt => opt.MapFrom(src => src.Description));

            CreateMap<Note, LogBook>()
             .ForMember(dest => dest.LogBookId, opt => opt.MapFrom(src => src.NoteId))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.NoteContent));


            CreateMap<Infrastructure.EntityModels.ActivityType, TypeOfActivity>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ActivityTypeId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Description));
            CreateMap<TypeOfActivity, Infrastructure.EntityModels.ActivityType>()
             .ForMember(dest => dest.ActivityTypeId, opt => opt.MapFrom(src => src.ID));
            

        }


    }
}
