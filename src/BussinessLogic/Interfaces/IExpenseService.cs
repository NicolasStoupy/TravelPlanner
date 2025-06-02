using BussinessLogic.Entities;
using Commons.ErrorsHandlings;

namespace BussinessLogic.Interfaces
{
    public interface IExpenseService
    {

        /// <summary>
        /// Adds a new cost entry to the specified activity.
        /// If the activity is not found, no changes are persisted.
        /// </summary>
        /// <param name="activityID">The identifier of the activity. Must be > 0.</param>
        /// <param name="newCost">The cost DTO to add. Must not be null.</param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(true) if the cost was added.
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) if input is invalid or the activity was not found.
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) on database error.
        ///   </description></item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool>> CreateCostAsync(int activityID, Cost newCost);

        /// <summary>
        /// Retrieves all cost entries for the specified activity.
        /// </summary>
        /// <param name="activityID">The identifier of the activity. Must be > 0.</param>
        /// <returns>
        /// A <see cref="ServiceResult{List{Cost}}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(list) with zero or more costs.
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) if the activityID is invalid or a database error occurs.
        ///   </description></item>
        /// </list>
        /// </returns>
        Task<ServiceResult<List<Cost>>> GetCostAsync(int activityID);

        /// <summary>
        /// Retrieves all available currency codes from the database.
        /// </summary>
        /// <returns>
        /// A <see cref="ServiceResult{List{String}}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(list) of currency codes (e.g. "USD", "EUR").
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) on database error.
        ///   </description></item>
        /// </list>
        /// </returns>
        ServiceResult<List<string>> GetCurrencies();

        /// <summary>
        /// Removes a cost entry and all its associated media.
        /// If the cost is not found, no changes are persisted.
        /// </summary>
        /// <param name="costID">The identifier of the cost to remove. Must be > 0.</param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(true) if the cost and its media were removed.
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) if the cost was not found or a database error occurs.
        ///   </description></item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool>> RemoveCostAsync(int costID);

        /// <summary>
        /// Removes a single ticket media file by GUID.
        /// </summary>
        /// <param name="ticketId">The GUID of the media file. Must be non‐empty.</param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(true) if the media record and file were removed.
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) if the media record was not found or a database/file error occurs.
        ///   </description></item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool>> RemoveTicketAsync(Guid ticketId);

        /// <summary>
        /// Saves a new media file for a cost under the specified travel.
        /// If saving the file or persisting the record fails, no state is changed.
        /// </summary>
        /// <param name="travelID">The ID of the travel. Must be > 0.</param>
        /// <param name="costID">The ID of the associated cost. Must be > 0.</param>
        /// <param name="file">The byte array of the file. Must not be null or empty.</param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(true) if the media was saved and recorded.
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) if input is invalid, file save fails, or database error occurs.
        ///   </description></item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool>> SaveNewCostAsync(int travelID, int costID, byte[] file);
        //List<string> GetCurrencies();

        //List<Cost> GetCost(int ActivityID);
        //Result SaveNewCost(int id, int activityID, byte[] file);
        //Result RemoveTicket(Guid ticketId);
        //Result RemoveCost(int costID);
        //Result CreateCost( int activityID, Cost newCost);
    }
}
