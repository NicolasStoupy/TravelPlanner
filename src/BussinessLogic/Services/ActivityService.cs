using AutoMapper;

using BussinessLogic.Interfaces;
using Commons.Models;
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using System.Collections.Generic;

namespace BussinessLogic.Services
{
    public class ActivityService(TravelPlannerContext context, IMapper mapper) : IActivityService
    {
        private readonly TravelPlannerContext _context = context;
        private readonly IMapper _mapper = mapper;



        public async Task<Result> SaveNewActivity(int travelID, Infrastructure.EntityModels.Activity newActivity)
        {
            try
            {
                var trip = _context.Trips.FirstOrDefault(t => t.TripId == travelID);
                if (trip != null)
                {
                    trip.Activities.Add(newActivity);
                    await _context.SaveChangesAsync();
                }
                return Result.Success("Activité sauvegarder aevc success");
            }
            catch (Exception ex)
            {
                return Result.Failure("Erreur lors de l'enregistrement : " + ex.Message);
            }
        }

        public Task<Result> UpdateActivity(Infrastructure.EntityModels.Activity travelActivity)
        {
            throw new NotImplementedException();
        }

        public List<Infrastructure.EntityModels.ActivityType> GetActivitiesTypes()
        {
            var activityType = _context.ActivityTypes;
            if (activityType != null)
            {
                return activityType.ToList();
            }

            return new List<Infrastructure.EntityModels.ActivityType>();
        }
    }
}

