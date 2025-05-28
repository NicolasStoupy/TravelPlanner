using BussinessLogic.Entities;
using BussinessLogic.Services.ServicesStatus;
using Commons;
using Commons.ErrorsHandlings;
using Commons.Models;
using System.Collections.ObjectModel;

namespace BussinessLogic.Interfaces
{
    public interface ITravelService
    {
        /// <summary>
        /// Retrieves the <see cref="Travel"/> with the given ID.
        /// </summary>
        /// <param name="travelID">The ID of the travel to fetch. Must be greater than zero.</param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <c>Travel</c> and <see cref="TravelServiceStatus"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="SuccessResult{T,TStatus}"/> containing the <c>Travel</c> if found.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.InvalidTravelId"/>
        ///       if <paramref name="travelID"/> is less than or equal to zero.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.TravelNotFound"/>
        ///       if no travel exists for the given ID.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.UnknownError"/>
        ///       if an unexpected exception occurs during retrieval.
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        ServiceResult<Travel, TravelServiceStatus> GetTravel(int travelID);

        /// <summary>
        /// Retrieves all trips from the database, maps them to <see cref="Travel"/> DTOs,
        /// and returns them in a <see cref="ServiceResult{T,TStatus}"/>.
        /// </summary>
        /// <param name="includeActivity">
        /// If <c>true</c>, related activity data should be included (not yet implemented).
        /// </param>
        /// <param name="includeNotes">
        /// If <c>true</c>, related notes data should be included (not yet implemented).
        /// </param>
        /// <param name="includeFollowers">
        /// If <c>true</c>, related follower data should be included (not yet implemented).
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <c>List&lt;Travel&gt;</c> and <see cref="TravelServiceStatus"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="SuccessResult{T,TStatus}"/> with the mapped list on success.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.UnknownError"/>
        ///       if the mapping operation fails.
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<List<Travel>, TravelServiceStatus>> GetTravels(bool includeActivity = false, bool includeNotes = false, bool includeFollowers = false);

        /// <summary>
        /// Saves the given media files to the specified travel record in the database.
        /// </summary>
        /// <param name="medias">A list of media file contents as byte arrays.</param>
        /// <param name="travelID">The ID of the travel record to which the media will be added.</param>
        /// <param name="mediaType">The category/type of the media being added.</param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <c>bool</c> and <see cref="TravelServiceStatus"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="SuccessResult{T,TStatus}"/> with <c>true</c> if all media were saved and linked successfully.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.InvalidTravelId"/>
        ///       if <paramref name="travelID"/> is less than or equal to zero.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.NoMedia"/>
        ///       if <paramref name="medias"/> is empty.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.TravelNotFound"/>
        ///       if no travel exists for the given ID.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.ErrorWhenAddingFile"/>
        ///       if some files failed to save (already saved files will be rolled back).</description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.UnknownError"/>
        ///       for any other unhandled exception.
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool, TravelServiceStatus>> AddMediaToTravel(List<byte[]> medias, int travelID, Commons.TypeMedia images);

        /// <summary>
        /// Creates a new <see cref="Travel"/> record (and optional image) in the database.
        /// All operations run inside a transaction: if image saving fails, the trip insert is rolled back.
        /// </summary>
        /// <param name="travel">
        /// The <see cref="Travel"/> DTO to save. Must not be null and should contain all required fields.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <c>bool</c> and <see cref="TravelServiceStatus"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="SuccessResult{T,TStatus}"/> with <c>true</c> and
        ///       <see cref="TravelServiceStatus.TravelCreated"/> on success.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.InvalidTravelId"/>
        ///       if <paramref name="travel"/> is null.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.UnknownError"/>
        ///       if mapping to <see cref="Trip"/> fails or any unexpected exception occurs.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.ErrorWhenAddingFile"/>
        ///       if saving the image file fails (the trip insert is rolled back).</description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool, TravelServiceStatus>> SaveTravel(Travel travel);

        /// <summary>
        /// Deletes the specified travel and all its related entities (media, log books, activities, attendees, and costs).
        /// </summary>
        /// <param name="travelID">The ID of the travel to delete. Must be zero or positive.</param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <c>bool</c> and <see cref="TravelServiceStatus"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="SuccessResult{T,TStatus}"/> with <c>true</c> if the travel and all related data were deleted successfully.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.InvalidTravel"/>
        ///       if <paramref name="travelID"/> is less than zero.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.TravelNotFound"/>
        ///       if no travel exists for the given <paramref name="travelID"/>.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.UnknownError"/>
        ///       if any unexpected error occurs during deletion.
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool, TravelServiceStatus>> DeleteTravel(int travelID);

        /// <summary>
        /// Updates an existing <see cref="Travel"/> record (and its optional image) in the database.
        /// All changes are wrapped in a transaction: if anything fails, database changes are rolled back
        /// and any newly saved image is removed.
        /// </summary>
        /// <param name="travel">
        /// The <see cref="Travel"/> DTO containing updated values. Must not be null and must have a valid <c>Id</c>.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{T, TStatus}"/> of <c>bool</c> and <see cref="TravelServiceStatus"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.InvalidTravel"/>
        ///       if <paramref name="travel"/> is null.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.UnknownError"/>
        ///       if mapping to <see cref="Trip"/> fails.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.TravelNotFound"/>
        ///       if no existing trip matches <c>travel.Id</c>.</description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="SuccessResult{T,TStatus}"/> with <c>true</c> if both the base data and optional image
        ///       replacement succeed.</description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ErrorResult{T,TStatus}"/> with <see cref="TravelServiceStatus.UnknownError"/>
        ///       for any other error, after rolling back the transaction and cleaning up any saved image.</description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool, TravelServiceStatus>> UpdateTravel(Travel travel);

        /// <summary>
        /// Clones the specified <see cref="Travel"/> by creating a new <see cref="Trip"/> record without its original ID.
        /// </summary>
        /// <param name="travel">
        /// The travel data to clone. If <c>null</c>, returns <see cref="TravelServiceStatus.InvalidTravelId"/>.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{T,TStatus}"/> of <c>bool</c> and <see cref="TravelServiceStatus"/>:
        /// <list type="bullet">
        ///   <item><description><c>true</c> on success.</description></item>
        ///   <item><description><see cref="TravelServiceStatus.DatabaseError"/> if saving fails.</description></item>
        ///   <item><description><see cref="TravelServiceStatus.UnknownError"/> for any other error.</description></item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool, TravelServiceStatus>> CloneTravel(Travel travel);  

        List<MemoryFile> GetMemories(int id, TypeMedia mediaType);

        Task<Result> RemoveMemories(IEnumerable<MemoryFile>? selectedMemories, int id);

        Result UpdateMemory(MemoryFile memory);

        // Updating
    }
}