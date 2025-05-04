using AutoMapper;
using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Commons.Models;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;


namespace BussinessLogic.Services
{
    public class ActivityService(IDbContextFactory<TravelPlannerContext> contextFactory, IMapper mapper) : IActivityService
    {
        private readonly IDbContextFactory<TravelPlannerContext> _contextFactory = contextFactory;
        private readonly IMapper _mapper = mapper;


        public async Task<Result> SaveNewActivity(TravelActivity newActivity)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var newActivityDb = _mapper.Map<Activity>(newActivity);

                newActivityDb.ActivityType = null;
                context.Activities.Add(newActivityDb);
                await context.SaveChangesAsync();

                return Result.Success("Activité sauvegarder aevc success");
            }
            catch (Exception ex)
            {
                return Result.Failure("Erreur lors de l'enregistrement : " + ex.Message);
            }
        }

        public Task<Result> UpdateActivity(TravelActivity travelActivity)
        {
            throw new NotImplementedException();
        }

        public List<TypeOfActivity> GetActivitiesTypes()
        {
            using var context = _contextFactory.CreateDbContext();

            var activityTypes = context.ActivityTypes;
            if (activityTypes != null)
            {
                var typeofActivities = _mapper.Map<List<TypeOfActivity>>(activityTypes);
                return typeofActivities;
            }
            return new List<TypeOfActivity>();

        }

        public List<TravelActivity> GetActivities(int travelID)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var activities = context.Activities.Where(a => a.TripId == travelID);
                var TravelActivities = _mapper.Map<List<TravelActivity>>(activities);
                return TravelActivities;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<Result> DeleteActivity(TravelActivity travelActivity)
        {
            using var context = _contextFactory.CreateDbContext();
            var activity = context.Activities.FirstOrDefault(a => a.ActivityId == travelActivity.ActivityID);
            if (activity != null)
            {
                context.Activities.Remove(activity);
                await context.SaveChangesAsync();
                return Result.Success("Activité supprimée");
            }
            else
            {
                return Result.Success("L'activité n'existe plus ");
            }               
          
        
        }
    }
}

