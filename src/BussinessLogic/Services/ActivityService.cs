using AutoMapper;
using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Commons.Models;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;


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
                var newSequenceForActivity = GetSequenceForActivity(newActivity.TravelID);
                newActivityDb.Sequence = newSequenceForActivity;
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

        private int GetSequenceForActivity(int travelID)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.Activities.Where(a => a.TripId == travelID);
            int maxSequence = query.Any()
                ? query.Max(a => a.Sequence)
                : 0;

            return maxSequence + 1;
        }

        public async Task<Result> UpdateActivity(TravelActivity travelActivity, int travelID)
        {
            using var context = _contextFactory.CreateDbContext();

            var activity = _mapper.Map<Activity>(travelActivity);
            activity.TripId = travelID;
            context.Activities.Update(activity);
            await context.SaveChangesAsync();
            return Result.Success("Success");
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
                foreach (var item in TravelActivities)
                {
                    item.Total = item.Cost.Sum(c => c.Price);
                }
                return TravelActivities.OrderBy(t => t.Sequence).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<Result> DeleteActivity(TravelActivity travelActivity)
        {
            using var context = _contextFactory.CreateDbContext();
            var activity = context.Activities
                .Include(i => i.ActivityCosts).ThenInclude(m => m.Media)
                .Include(f => f.Attendees)
                .FirstOrDefault(a => a.ActivityId == travelActivity.ActivityID);
            if (activity != null)
            {
                var medias = activity.ActivityCosts.SelectMany(f => f.Media);
                var attendees = activity.Attendees;
                context.Attendees.RemoveRange(attendees);
                context.Media.RemoveRange(medias);
                context.ActivityCosts.RemoveRange(activity.ActivityCosts);
                context.Activities.Remove(activity);
                await context.SaveChangesAsync();
                return Result.Success("Activité supprimée");
            }
            else
            {
                return Result.Success("L'activité n'existe plus ");
            }



        }

        public TravelActivity? GetActivity(int travelActivityID)
        {
            using var context = _contextFactory.CreateDbContext();
            var activity = context.Activities.FirstOrDefault(a => a.ActivityId == travelActivityID);
            var travelActivity = _mapper.Map<TravelActivity>(activity);
            return travelActivity;
        }

        public async Task<bool> UpdateSequence(ObservableCollection<TravelActivity>? activities)
        {
            if (activities == null || activities.Count == 0)
                return false;

            using var context = _contextFactory.CreateDbContext();

            // Démarre une transaction 
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // 1) Passe 1 : valeurs temporaires uniques (-(i+1))
                for (int i = 0; i < activities.Count; i++)
                {
                    var id = activities[i].ActivityID;
                    var dbAct = await context.Activities
                                             .FirstOrDefaultAsync(a => a.ActivityId == id);
                    if (dbAct != null)
                        dbAct.Sequence = -(i + 1);
                }
                await context.SaveChangesAsync();

                // 2) Passe 2 : séquences “réelles” 1,2,3…
                for (int i = 0; i < activities.Count; i++)
                {
                    var id = activities[i].ActivityID;
                    var dbAct = await context.Activities
                                             .FirstOrDefaultAsync(a => a.ActivityId == id);
                    if (dbAct != null)
                        dbAct.Sequence = i + 1;
                }
                await context.SaveChangesAsync();

                // Valide la transaction
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                // Annule tout si erreur
                await transaction.RollbackAsync();
                throw;
            }
        }

        TravelActivity IActivityService.GetActivity(int activityID)
        {
            using var context = _contextFactory.CreateDbContext();
            var Activity = context.Activities.Single(a => a.ActivityId == activityID);

            var travelActivity = _mapper.Map<TravelActivity>(Activity);
            travelActivity.Total = travelActivity.Cost.Sum(c => c.Price);
            return travelActivity;

        }

        public Task<bool> AddFollower(int activityID, Follower follower)
        {
            using var context = _contextFactory.CreateDbContext();
            var activity = context.Activities.FirstOrDefault(a => a.ActivityId == activityID);
            if (activity != null)
            {

                var attendee = _mapper.Map<Attendee>(follower);

                activity.Attendees.Add(attendee);

                context.SaveChanges();
            }
            return Task.FromResult(true);
        }

        public List<Follower>? GetFollowers(int activityID)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Activities.Include(a => a.Attendees)
                .Where(a => a.ActivityId == activityID).ToList();
            throw new NotImplementedException();
        }

        public Task<bool> RemoveFollower(Follower follower, int activityID)
        {
            using var context = _contextFactory.CreateDbContext();

            var activity = context.Activities.Include(i=>i.Attendees).FirstOrDefault(a => a.ActivityId == activityID);
            if (activity != null)
            {
                var attendee = activity.Attendees.FirstOrDefault(a => a.AttendeeId == follower.FollowerID);

                if (attendee != null)
                {
                    context.Attendees.Remove(attendee);
                    context.SaveChanges();
                }
            }
            return Task.FromResult(true);
        }
    }
}

