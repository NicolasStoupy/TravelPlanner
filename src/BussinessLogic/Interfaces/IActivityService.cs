using BussinessLogic.Entities;


using Commons.Models;

namespace BussinessLogic.Interfaces
{
    public interface IActivityService
    {
        /// <summary>
        /// Deletes the given activity and its related data.
        /// </summary>
        /// <param name="activity">The activity DTO to delete.</param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/> indicating success or failure.
        /// </returns>
        Task<ServiceResult<bool>> DeleteActivity(TravelActivity activity);

        /// <summary>
        /// Retrieves all activities for the specified travel.
        /// </summary>
        /// <param name="travelID">The ID of the travel.</param>
        /// <returns>
        /// A <see cref="ServiceResult{List{TravelActivity}}"/> containing the list of activities.
        /// </returns>
        Task<ServiceResult<List<TravelActivity>>> GetActivities(int travelID);

        /// <summary>
        /// Gets the list of available activity types.
        /// </summary>
        /// <returns>
        /// A <see cref="ServiceResult{List{TypeOfActivity}}"/> with all activity types.
        /// </returns>
        ServiceResult<List<TypeOfActivity>> GetActivitiesTypes();

        /// <summary>
        /// Saves a new activity.
        /// </summary>
        /// <param name="newActivity">The activity DTO to create.</param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/> indicating success or failure.
        /// </returns>
        Task<ServiceResult<bool>> SaveNewActivity(TravelActivity newActivity);

        /// <summary>
        /// Updates an existing activity.
        /// </summary>
        /// <param name="travelActivity">The activity DTO with updated data.</param>
        /// <param name="travelID">The ID of the travel it belongs to.</param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/> indicating success or failure.
        /// </returns>
        Task<ServiceResult<bool>> UpdateActivity(TravelActivity travelActivity, int travelID);

        /// <summary>
        /// Retrieves a single activity by its ID.
        /// </summary>
        /// <param name="activityID">The ID of the activity.</param>
        /// <returns>
        /// A <see cref="ServiceResult{TravelActivity}"/> containing the activity data.
        /// </returns>
        ServiceResult<TravelActivity> GetActivity(int activityID);

        /// <summary>
        /// Reorders activities by updating their sequence values.
        /// </summary>
        /// <param name="activities">
        /// The ordered list of activities to apply.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/> indicating success or failure.
        /// </returns>
        Task<ServiceResult<bool>> UpdateSequence(List<TravelActivity>? activities);

        /// <summary>
        /// Adds a follower to an activity.
        /// </summary>
        /// <param name="activityID">The ID of the activity.</param>
        /// <param name="follower">The follower DTO to add.</param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/> indicating success or failure.
        /// </returns>
        Task<ServiceResult<bool>> AddFollower(int activityID, Follower follower);

        /// <summary>
        /// Retrieves followers of a given activity.
        /// </summary>
        /// <param name="activityID">The ID of the activity.</param>
        /// <returns>
        /// A <see cref="ServiceResult{List{Follower}}"/> with the list of followers.
        /// </returns>
        Task<ServiceResult<List<Follower>>> GetFollowers(int activityID);

        /// <summary>
        /// Removes a follower from an activity.
        /// </summary>
        /// <param name="follower">The follower DTO to remove.</param>
        /// <param name="activityID">The ID of the activity.</param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/> indicating success or failure.
        /// </returns>
        Task<ServiceResult<bool>> RemoveFollower(Follower follower, int activityID);
    }

}