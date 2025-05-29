using BussinessLogic.Entities;
using BussinessLogic.Services.ServicesStatus;
using Commons.ErrorsHandlings;

namespace BussinessLogic.Interfaces
{
    public interface IActivityService
    {
        /// <summary>
        /// Deletes the specified activity and all its related data (media, attendees, costs).
        /// </summary>
        /// <param name="travelActivity">
        /// The <see cref="TravelActivity"/> DTO identifying the activity to delete. Must not be <c>null</c>.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <c>bool</c> and <see cref="ActivityServiceStatus"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="SuccessResult{T,TStatus}"/> with <c>true</c> if the activity was deleted
        ///       or did not exist.  
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.InvalidActivity"/>
        ///       if <paramref name="travelActivity"/> is <c>null</c> or its <c>ActivityID</c> is not valid.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.PersistenceError"/>
        ///       if a <see cref="DbUpdateException"/> occurs during deletion.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.UnknownError"/>
        ///       for any other unexpected exception.
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool, ActivityServiceStatus>> DeleteActivity(TravelActivity activity);

        /// <summary>
        /// Retrieves all activities for the specified travel, computes each activity’s total cost,
        /// and returns them ordered by sequence.
        /// </summary>
        /// <param name="travelID">
        /// The ID of the travel whose activities should be fetched. Must be greater than zero.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <c>List&lt;TravelActivity&gt;</c> and <see cref="ActivityServiceStatus"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description><see cref="SuccessResult{T,TStatus}"/> with a list of mapped <see cref="TravelActivity"/> on success (empty list if none).</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.InvalidActivity"/>
        ///     if <paramref name="travelID"/> is not valid (≤ 0).</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.ActivityNotFound"/>
        ///     if no activities are found for the given travel.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.MappingError"/>
        ///     if mapping to <see cref="TravelActivity"/> fails.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.UnknownError"/>
        ///     for any other unexpected exception.</description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<List<TravelActivity>, ActivityServiceStatus>> GetActivities(int travelID);

        /// <summary>
        /// Retrieves all available activity types.
        /// </summary>
        /// <returns>A list of <see cref="TypeOfActivity"/> defined in the system.</returns>
        ServiceResult<List<TypeOfActivity>, ActivityServiceStatus> GetActivitiesTypes();

        /// <summary>
        /// Saves a new activity for a given travel.
        /// </summary>
        /// <param name="newActivity">
        /// The <see cref="TravelActivity"/> DTO to create. Must not be <c>null</c>.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <c>bool</c> and <see cref="ActivityServiceStatus"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description><see cref="SuccessResult{T,TStatus}"/> with <c>true</c> if the activity is created successfully.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.InvalidActivity"/>
        ///     if <paramref name="newActivity"/> is <c>null</c>.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.MappingError"/>
        ///     if AutoMapper mapping fails.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.PersistenceError"/>
        ///     if a <see cref="DbUpdateException"/> occurs during database save.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.UnknownError"/>
        ///     for any other unexpected exception.</description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool, ActivityServiceStatus>> SaveNewActivity(TravelActivity newActivity);

        /// <summary>
        /// Updates an existing activity under the specified travel.
        /// </summary>
        /// <param name="travelActivity">
        /// The <see cref="TravelActivity"/> DTO containing updated data. Must not be <c>null</c>.
        /// </param>
        /// <param name="travelID">
        /// The ID of the travel to which this activity belongs. Must be greater than zero.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <c>bool</c> and <see cref="ActivityServiceStatus"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description><see cref="SuccessResult{T,TStatus}"/> with <c>true</c> if the update succeeds.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.InvalidActivity"/>
        ///     if <paramref name="travelActivity"/> is <c>null</c>.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.MappingError"/>
        ///     if mapping via AutoMapper fails.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.PersistenceError"/>
        ///     if a <see cref="DbUpdateException"/> occurs while saving.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.UnknownError"/>
        ///     for any other unexpected exception.</description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool, ActivityServiceStatus>> UpdateActivity(TravelActivity travelActivity, int travelID);

        /// <summary>
        /// Retrieves the <see cref="TravelActivity"/> for the given activity ID.
        /// </summary>
        /// <param name="activityID">The ID of the activity to fetch. Must be greater than zero.</param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <see cref="TravelActivity"/> and <see cref="ActivityServiceStatus"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description><see cref="SuccessResult{T,TStatus}"/> with the mapped <see cref="TravelActivity"/> if found.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.InvalidActivity"/>
        ///     if <paramref name="activityID"/> is not valid (≤ 0).</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.ActivityNotFound"/>
        ///     if no activity exists with that ID.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.MappingError"/>
        ///     if AutoMapper mapping fails.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.UnknownError"/>
        ///     for any other unexpected exception.</description>
        ///   </item>
        /// </list>
        /// </returns>
        ServiceResult<TravelActivity, ActivityServiceStatus> GetActivity(int activityID);

        /// <summary>
        /// Updates the sequence order of the given activities in a transactional manner.
        /// </summary>
        /// <param name="activities">
        /// The ordered collection of <see cref="TravelActivity"/> whose <c>Sequence</c> values will be updated.
        /// Must not be null or empty.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <c>bool</c> and <see cref="ActivityServiceStatus"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description><see cref="SuccessResult{T,TStatus}"/> with <c>true</c> if reordering succeeds.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.InvalidActivity"/>
        ///     if <paramref name="activities"/> is null or empty.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.PersistenceError"/>
        ///     if a database update error occurs.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.UnknownError"/>
        ///     for any other unexpected exception.</description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool, ActivityServiceStatus>> UpdateSequence(List<TravelActivity>? activities);

        /// <summary>
        /// Adds a follower to the specified activity.
        /// </summary>
        /// <param name="activityID">The ID of the activity to which the follower will be added. Must be greater than zero.</param>
        /// <param name="follower">The <see cref="Follower"/> DTO to add. Must not be null.</param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <c>bool</c> and <see cref="ActivityServiceStatus"/>:
        /// <list type="bullet">
        ///   <item><description><see cref="SuccessResult{T,TStatus}"/> with <c>true</c> if the follower is added successfully.</description></item>
        ///   <item><description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.InvalidActivity"/>
        ///     if <paramref name="activityID"/> is not valid (≤ 0).</description></item>
        ///   <item><description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.MappingError"/>
        ///     if mapping the <paramref name="follower"/> to <see cref="Attendee"/> fails.</description></item>
        ///   <item><description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.ActivityNotFound"/>
        ///     if no activity exists for that ID.</description></item>
        ///   <item><description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.PersistenceError"/>
        ///     if a <see cref="DbUpdateException"/> occurs while saving.</description></item>
        ///   <item><description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.UnknownError"/>
        ///     for any other unexpected exception.</description></item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool, ActivityServiceStatus>> AddFollower(int activityID, Follower follower);

        /// <summary>
        /// Retrieves the list of followers for the specified activity.
        /// </summary>
        /// <param name="activityID">
        /// The ID of the activity whose followers should be fetched. Must be greater than zero.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <c>List&lt;Follower&gt;</c> and <see cref="ActivityServiceStatus"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description><see cref="SuccessResult{T,TStatus}"/> with the followers if the activity exists (empty list if no followers).</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.InvalidActivity"/>
        ///     if <paramref name="activityID"/> is not valid (≤ 0).</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.ActivityNotFound"/>
        ///     if no activity exists for the given ID.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.MappingError"/>
        ///     if mapping the attendees to <see cref="Follower"/> fails.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.PersistenceError"/>
        ///     if a database error occurs while fetching.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.UnknownError"/>
        ///     for any other unexpected exception.</description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<List<Follower>, ActivityServiceStatus>> GetFollowers(int activityID);

        /// <summary>
        /// Removes the specified follower from the given activity.
        /// </summary>
        /// <param name="follower">
        /// The <see cref="Follower"/> DTO representing the attendee to remove. Must not be <c>null</c>.
        /// </param>
        /// <param name="activityID">
        /// The ID of the activity from which to remove the follower. Must be greater than zero.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <c>bool</c> and <see cref="ActivityServiceStatus"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="SuccessResult{T,TStatus}"/> with <c>true</c> if the follower was removed or did not exist.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.InvalidActivity"/>
        ///       if <paramref name="follower"/> is <c>null</c> or <paramref name="activityID"/> is not valid (≤ 0).
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.ActivityNotFound"/>
        ///       if no activity exists for the given <paramref name="activityID"/>.</description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.PersistenceError"/>
        ///       if a <see cref="DbUpdateException"/> occurs while saving changes.</description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="ActivityServiceStatus.UnknownError"/>
        ///       for any other unexpected exception.</description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool, ActivityServiceStatus>> RemoveFollower(Follower follower, int activityID);
    }
}