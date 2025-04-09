using BussinessLogic.Entities;
using BussinessLogic.Models;
using Infrastructure.EntityModels;

namespace BussinessLogic.Interfaces
{
    public interface ITravelService
    {
        List<TravelItem> GetTravelItems();

        /// <summary>
        /// Saves a new trip to the database and optionally stores the associated image as a separate document.
        /// </summary>
        /// <param name="trip">The trip entity to be persisted in the database.</param>
        /// <param name="image">The optional image file to be stored after the trip is saved.</param>
        /// <returns>
        /// A <see cref="Task{Result}"/> representing the asynchronous operation.
        /// Returns <c>Result.Success()</c> if the operation completes successfully, or <c>Result.Failure(string)</c> if an exception is thrown.
        /// </returns>
        /// <remarks>
        /// This method first attempts to save the trip to the database. If successful, and an image is provided, the image is stored using the document service.
        /// The trip's <c>TripBackgroundGuid</c> is then updated with the document ID, followed by a second database save operation.
        /// </remarks>
        Task<Result> SaveTrip(Trip travel, byte[]? image);
    }
}