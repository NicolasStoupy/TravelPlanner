using AutoMapper;
using BussinessLogic.Entities;
using BussinessLogic.Extensions;
using BussinessLogic.Interfaces;
using BussinessLogic.Services.ServicesStatus;
using Commons.ErrorsHandlings;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BussinessLogic.Services
{
    public class ActivityService(IDbContextFactory<TravelPlannerContext> contextFactory, IMapper mapper, ILogger<ActivityService> logger) : IActivityService
    {
        private readonly IDbContextFactory<TravelPlannerContext> _contextFactory = contextFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ActivityService> _logger = logger;

        public async Task<ServiceResult<bool, ActivityServiceStatus>> SaveNewActivity(TravelActivity newActivity)
        {
            // 1. Input validation
            if (newActivity is null)
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.InvalidActivity);

            // 2. Mapping
            if (!_mapper.TryMap(newActivity, out Activity entity, _logger))
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.MappingError);

            // 3. Business logic
            entity.Sequence = GetSequenceForActivity(newActivity.TravelID);
            entity.ActivityType = default!;

            try
            {
                await using var ctx = _contextFactory.CreateDbContext();
                ctx.Activities.Add(entity);
                await ctx.SaveChangesAsync();

                return new SuccessResult<bool, ActivityServiceStatus>(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx,
                    "DB error saving activity for TravelID={TravelID}",
                    newActivity.TravelID);
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.PersistenceError);
            }
          
        }

        /// <summary>
        /// Calculates the next sequence number for an activity under the given travel.
        /// It finds the current highest sequence among existing activities and returns that value plus one.
        /// </summary>
        /// <param name="travelID">
        /// The identifier of the travel whose activity sequence is being generated.
        /// </param>
        /// <returns>
        /// The next sequence number (i.e. max existing sequence + 1).
        /// If there are no existing activities, returns 1.
        /// </returns>
        private int GetSequenceForActivity(int travelID)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.Activities.Where(a => a.TripId == travelID);
            int maxSequence = query.Any()
                ? query.Max(a => a.Sequence)
                : 0;

            return maxSequence + 1;
        }

        public async Task<ServiceResult<bool, ActivityServiceStatus>> UpdateActivity(
            TravelActivity travelActivity, int travelID)
        {
            // 1. Validate input
            if (travelActivity is null)
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.InvalidActivity);
            if (travelID <= 0)
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.InvalidActivity);

            // 2. Map DTO to entity
            if (!_mapper.TryMap(travelActivity, out Activity entity, _logger))
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.MappingError);

            entity.TripId = travelID;

            try
            {
                await using var ctx = _contextFactory.CreateDbContext();
                ctx.Activities.Update(entity);
                await ctx.SaveChangesAsync();
                return new SuccessResult<bool, ActivityServiceStatus>(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx,
                    "Database error updating activity {ActivityID}", travelActivity.ActivityID);
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.PersistenceError);
            }
           
        }


        public ServiceResult<List<TypeOfActivity>, ActivityServiceStatus> GetActivitiesTypes()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var entities = context.ActivityTypes.ToList();

                if (!_mapper.TryMap(entities, out List<TypeOfActivity> dtoList, _logger))
                {
                    return new ErrorResult<List<TypeOfActivity>, ActivityServiceStatus>(
                        ActivityServiceStatus.MappingError);
                }

                return new SuccessResult<List<TypeOfActivity>, ActivityServiceStatus>(dtoList);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error fetching activity types");
                return new ErrorResult<List<TypeOfActivity>, ActivityServiceStatus>(
                    ActivityServiceStatus.PersistenceError);
            }
           
        }
        public async Task<ServiceResult<List<TravelActivity>, ActivityServiceStatus>> GetActivities(int travelID)
        {
            if (travelID <= 0)
                return new ErrorResult<List<TravelActivity>, ActivityServiceStatus>(
                    ActivityServiceStatus.InvalidActivity);

            await using var ctx = _contextFactory.CreateDbContext();
            var entities = await ctx.Activities
                .Where(a => a.TripId == travelID)
                .ToListAsync();

            if (entities == null || entities.Count == 0)
                return new SuccessResult<List<TravelActivity>, ActivityServiceStatus>(new List<TravelActivity>());

            if (!_mapper.TryMap(entities, out List<TravelActivity> dtos, _logger))
                return new ErrorResult<List<TravelActivity>, ActivityServiceStatus>(
                    ActivityServiceStatus.MappingError);

            foreach (var activity in dtos)
                activity.Total = activity.Cost.Sum(c => c.Price);

            var ordered = dtos.OrderBy(a => a.Sequence).ToList();
            return new SuccessResult<List<TravelActivity>, ActivityServiceStatus>(ordered);

        }

        public async Task<ServiceResult<bool, ActivityServiceStatus>> DeleteActivity(TravelActivity travelActivity)
        {
            try { 
            if (travelActivity is null || travelActivity.ActivityID <= 0)
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.InvalidActivity);
            
                await using var ctx = _contextFactory.CreateDbContext();

                var entity = await ctx.Activities
                    .Include(a => a.ActivityCosts).ThenInclude(c => c.Media)
                    .Include(a => a.Attendees)
                    .FirstOrDefaultAsync(a => a.ActivityId == travelActivity.ActivityID);

                if (entity != null)
                {
                    // remove related records
                    ctx.Attendees.RemoveRange(entity.Attendees);
                    ctx.Media.RemoveRange(entity.ActivityCosts.SelectMany(c => c.Media));
                    ctx.ActivityCosts.RemoveRange(entity.ActivityCosts);
                    ctx.Activities.Remove(entity);

                    await ctx.SaveChangesAsync();
                }

                // If entity was null, we consider it already "deleted"
                return new SuccessResult<bool, ActivityServiceStatus>(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx,
                    "Database error deleting activity {ActivityID}",
                    travelActivity.ActivityID);
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.PersistenceError);
            }
          
        }
        public async Task<ServiceResult<bool, ActivityServiceStatus>> UpdateSequence(List<TravelActivity>? activities)
        {
            // 1. Input validation
            if (activities == null || activities.Count == 0)
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.InvalidActivity);

            await using var ctx = _contextFactory.CreateDbContext();
            await using var transaction = await ctx.Database.BeginTransactionAsync();

            try
            {
                // Pass 1: assign temporary negative sequences
                for (int i = 0; i < activities.Count; i++)
                {
                    var id = activities[i].ActivityID;
                    var dbAct = await ctx.Activities
                                         .FirstOrDefaultAsync(a => a.ActivityId == id);
                    if (dbAct != null)
                        dbAct.Sequence = -(i + 1);
                }
                await ctx.SaveChangesAsync();

                // Pass 2: assign final positive sequences
                for (int i = 0; i < activities.Count; i++)
                {
                    var id = activities[i].ActivityID;
                    var dbAct = await ctx.Activities
                                         .FirstOrDefaultAsync(a => a.ActivityId == id);
                    if (dbAct != null)
                        dbAct.Sequence = i + 1;
                }
                await ctx.SaveChangesAsync();

                await transaction.CommitAsync();
                return new SuccessResult<bool, ActivityServiceStatus>(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error reordering activities");
                await transaction.RollbackAsync();
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.PersistenceError);
            }           
        }

        public ServiceResult<TravelActivity, ActivityServiceStatus> GetActivity(int activityID)
        {
            if (activityID <= 0)
                return new ErrorResult<TravelActivity, ActivityServiceStatus>(
                    ActivityServiceStatus.InvalidActivity);
            
                using var ctx = _contextFactory.CreateDbContext();
                var entity = ctx.Activities
                                .Include(a => a.ActivityCosts).ThenInclude(c => c.Media)
                                .Include(a => a.Attendees)
                                .SingleOrDefault(a => a.ActivityId == activityID);

                if (entity == null)
                    return new ErrorResult<TravelActivity, ActivityServiceStatus>(
                        ActivityServiceStatus.ActivityNotFound);

                if (!_mapper.TryMap(entity, out TravelActivity result, _logger))
                    return new ErrorResult<TravelActivity, ActivityServiceStatus>(
                        ActivityServiceStatus.MappingError);

                result.Total = result.Cost.Sum(c => c.Price);
                return new SuccessResult<TravelActivity, ActivityServiceStatus>(result);
      
        }

        public async Task<ServiceResult<bool, ActivityServiceStatus>> AddFollower(int activityID, Follower follower)
        {
            // 1. Validate inputs
            if (activityID <= 0)
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.InvalidActivity);
            if (follower is null)
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.ErrorWhenAddingAttendee);

            // 2. Map DTO to entity
            if (!_mapper.TryMap(follower, out Attendee attendee, _logger))
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.MappingError);

            try
            {
                await using var ctx = _contextFactory.CreateDbContext();

                // 3. Find activity
                var activity = await ctx.Activities
                    .Include(a => a.Attendees)
                    .FirstOrDefaultAsync(a => a.ActivityId == activityID);
                if (activity == null)
                    return new ErrorResult<bool, ActivityServiceStatus>(
                        ActivityServiceStatus.ActivityNotFound);

                // 4. Add and save
                activity.Attendees.Add(attendee);
                await ctx.SaveChangesAsync();

                return new SuccessResult<bool, ActivityServiceStatus>(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx,
                    "Database error adding follower to activity {ActivityID}", activityID);
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.PersistenceError);
            }
          
        }
        public async Task<ServiceResult<List<Follower>, ActivityServiceStatus>> GetFollowers(int activityID)
        {
            if (activityID <= 0)
                return new ErrorResult<List<Follower>, ActivityServiceStatus>(
                    ActivityServiceStatus.InvalidActivity);

            try
            {
                await using var ctx = _contextFactory.CreateDbContext();
                var activity = await ctx.Activities
                    .Include(a => a.Attendees)
                    .FirstOrDefaultAsync(a => a.ActivityId == activityID);

                if (activity == null)
                    return new ErrorResult<List<Follower>, ActivityServiceStatus>(
                        ActivityServiceStatus.ActivityNotFound);

                var attendees = activity.Attendees.ToList();
                if (!_mapper.TryMap(attendees, out List<Follower> followers, _logger))
                    return new ErrorResult<List<Follower>, ActivityServiceStatus>(
                        ActivityServiceStatus.MappingError);

                return new SuccessResult<List<Follower>, ActivityServiceStatus>(followers);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error fetching followers for ActivityID={ActivityID}", activityID);
                return new ErrorResult<List<Follower>, ActivityServiceStatus>(
                    ActivityServiceStatus.PersistenceError);
            }
          
        }

        public async Task<ServiceResult<bool, ActivityServiceStatus>> RemoveFollower(Follower follower, int activityID)
        {
            // 1. Validate input
            if (follower is null || activityID <= 0)
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.InvalidActivity);

            try
            {
                await using var ctx = _contextFactory.CreateDbContext();

                // 2. Find the activity including its attendees
                var activity = await ctx.Activities
                    .Include(a => a.Attendees)
                    .FirstOrDefaultAsync(a => a.ActivityId == activityID);

                if (activity == null)
                    return new ErrorResult<bool, ActivityServiceStatus>(
                        ActivityServiceStatus.ActivityNotFound);

                // 3. Locate the attendee
                var attendee = activity.Attendees
                    .FirstOrDefault(a => a.AttendeeId == follower.FollowerID);

                // 4. Remove if found
                if (attendee != null)
                {
                    ctx.Attendees.Remove(attendee);
                    await ctx.SaveChangesAsync();
                }

                return new SuccessResult<bool, ActivityServiceStatus>(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx,
                    "Database error removing follower {FollowerID} from activity {ActivityID}",
                    follower.FollowerID, activityID);
                return new ErrorResult<bool, ActivityServiceStatus>(
                    ActivityServiceStatus.PersistenceError);
            }          
        }

    }
}