using AutoMapper;

using BussinessLogic.Interfaces;
using Commons.Models;
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BussinessLogic.Services
{
    public class ActivityService(IDbContextFactory<TravelPlannerContext> contextFactory, IMapper mapper) : IActivityService
    {
        private readonly IDbContextFactory<TravelPlannerContext> _contextFactory = contextFactory;
        private readonly IMapper _mapper = mapper;


        public async Task<Result> SaveNewActivity(int travelID, Infrastructure.EntityModels.Activity newActivity)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var trip = context.Trips.FirstOrDefault(t => t.TripId == travelID);
                if (trip != null)
                {
                    trip.Activities.Add(newActivity);
                    await context.SaveChangesAsync();
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
            using var context = _contextFactory.CreateDbContext();
            var activityType = context.ActivityTypes;
            if (activityType != null)
            {
                return activityType.ToList();
            }

            return new List<Infrastructure.EntityModels.ActivityType>();
        }

        public List<Activity> GetActivities(int travelID)
        {
            using var context = _contextFactory.CreateDbContext();
            var activities = context.Activities.Where(a=>a.TripId== travelID);
            if (activities != null)
                return activities.ToList();

            return new List<Activity>();
        }
    }
}

