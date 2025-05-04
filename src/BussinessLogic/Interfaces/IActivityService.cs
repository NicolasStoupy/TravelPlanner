using BussinessLogic.Entities;
using Commons.Models;

namespace BussinessLogic.Interfaces
{
    public interface IActivityService
    {
        /// <summary>
        /// Deletes a specific activity from the travel.
        /// </summary>
        /// <param name="activity">The activity to delete.</param>
        /// <returns>
        /// A <see cref="Result"/> indicating whether the operation was successful or failed.
        /// </returns>
        Task<Result> DeleteActivity(TravelActivity activity);

        /// <summary>
        /// Retrieves all activities associated with a given travel ID.
        /// </summary>
        /// <param name="travelID">The ID of the travel to fetch activities for.</param>
        /// <returns>A list of <see cref="TravelActivity"/> related to the specified travel.</returns>
        List<TravelActivity> GetActivities(int travelID);

        /// <summary>
        /// Retrieves all available activity types.
        /// </summary>
        /// <returns>A list of <see cref="TypeOfActivity"/> defined in the system.</returns>
        List<TypeOfActivity> GetActivitiesTypes();

        /// <summary>
        /// Saves a new activity for a travel.
        /// </summary>
        /// <param name="newActivity">The new activity to save.</param>
        /// <returns>A <see cref="Result"/> indicating whether the save operation was successful or failed.</returns>
        Task<Result> SaveNewActivity(TravelActivity newActivity);

        /// <summary>
        /// Updates an existing activity for a travel.
        /// </summary>
        /// <param name="travelActivity">The activity object with updated values.</param>
        /// <returns>A <see cref="Result"/> indicating whether the update operation was successful or failed.</returns>
        Task<Result> UpdateActivity(TravelActivity travelActivity);
    }
}