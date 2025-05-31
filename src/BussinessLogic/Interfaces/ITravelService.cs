using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BussinessLogic.Entities;
using Commons;
using Commons.Models;

namespace BussinessLogic.Interfaces
{
    /// <summary>
    /// Provides business operations for managing <see cref="Travel"/> records,
    /// media files, notes and related data.  
    /// All methods return a <see cref="ServiceResult{T}"/>, and in the event of failure
    /// any partial changes are rolled back so that the system state is unchanged.
    /// </summary>
    public interface ITravelService
    {
        /// <summary>
        /// Asynchronously adds one or more media files to the specified travel record.
        /// If any file fails to save, previously saved files in this operation are removed
        /// and the travel’s media collection remains unchanged.
        /// </summary>
        /// <param name="medias">
        /// A list of byte arrays, each representing a media file to attach. Must not be empty.
        /// </param>
        /// <param name="travelID">
        /// The identifier of the travel record. Must be greater than zero.
        /// </param>
        /// <param name="images">
        /// The media type (e.g., image, video, document).
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(true) if all files saved and linked successfully.
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) if the travel ID is invalid, no files were provided,
        ///     the travel wasn’t found, or any file save error occurred—
        ///     in which case no media remain attached.
        ///   </description></item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool>> AddMediaToTravel(
            List<byte[]> medias,
            int travelID,
            Commons.TypeMedia images);

        /// <summary>
        /// Asynchronously creates a duplicate of the given <see cref="Travel"/> record.
        /// On mapping or database errors, no new record is persisted.
        /// </summary>
        /// <param name="travel">
        /// The <see cref="Travel"/> to clone. Must not be null.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(true) if clone persisted.
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) if input is null, mapping fails, or persistence error—
        ///     no changes are made on failure.
        ///   </description></item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool>> CloneTravel(Travel travel);

        /// <summary>
        /// Asynchronously deletes the specified travel and all its related entities
        /// (activities, media, notes, etc.).  
        /// If any media file fails to delete, already‐deleted files are restored.
        /// </summary>
        /// <param name="travelID">
        /// The identifier of the travel to delete. Must be zero or greater.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(true) with message TRAVEL_REMOVED if deletion succeeds.
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) if the ID is invalid, travel not found, or any file deletion error—
        ///     state is restored on failure.
        ///   </description></item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool>> DeleteTravel(int travelID);

        /// <summary>
        /// Retrieves all memory files (standalone media) for the specified travel.
        /// </summary>
        /// <param name="id">
        /// The identifier of the travel. Must be greater than zero.
        /// </param>
        /// <param name="mediaType">
        /// The type of media to retrieve.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{List{MemoryFile}}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(list) containing zero or more <see cref="MemoryFile"/> entries.
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) if the travel ID is invalid or not found, or an unexpected error occurs.
        ///   </description></item>
        /// </list>
        /// </returns>
        ServiceResult<List<MemoryFile>> GetMemories(int id, TypeMedia mediaType);

        /// <summary>
        /// Retrieves a single <see cref="Travel"/> by its identifier.
        /// </summary>
        /// <param name="travelID">
        /// The identifier of the travel. Must be greater than zero.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Travel}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(travel) if found and mapped.
        ///   </description></item>
        ///   <item><description>
        ///     Warning(...) if input is invalid, not found, or mapping fails.
        ///   </description></item>
        /// </list>
        /// </returns>
        ServiceResult<Travel> GetTravel(int travelID);

        /// <summary>
        /// Asynchronously retrieves all <see cref="Travel"/> records, ordered by creation date.
        /// </summary>
        /// <param name="includeActivity">
        /// If true, includes related activity data (future use).
        /// </param>
        /// <param name="includeNotes">
        /// If true, includes related notes data (future use).
        /// </param>
        /// <param name="includeFollowers">
        /// If true, includes follower data (future use).
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{List{Travel}}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(list) of travels if mapping succeeds.
        ///   </description></item>
        ///   <item><description>
        ///     Warning(...) if an error occurs during mapping.
        ///   </description></item>
        /// </list>
        /// </returns>
        Task<ServiceResult<List<Travel>>> GetTravels(
            bool includeActivity = false,
            bool includeNotes = false,
            bool includeFollowers = false);

        /// <summary>
        /// Asynchronously removes specified <see cref="MemoryFile"/> entries from a travel.
        /// Partial deletions are committed, but any individual file deletion failure
        /// does not roll back the entire collection.
        /// </summary>
        /// <param name="selectedMemories">
        /// The memories to remove. Must not be null or empty.
        /// </param>
        /// <param name="id">
        /// The identifier of the travel. Must be greater than zero.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(true) if all specified memories are removed (or none were specified).
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) if the travel ID is invalid, not found, or deletion errors occur.
        ///   </description></item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool>> RemoveMemories(
            IEnumerable<MemoryFile>? selectedMemories,
            int id);

        /// <summary>
        /// Asynchronously saves a new <see cref="Travel"/>, including its optional image.
        /// Uses a database transaction to ensure atomicity;
        /// on any failure, the database is rolled back and any saved file is removed.
        /// </summary>
        /// <param name="travel">
        /// The <see cref="Travel"/> to save. Must not be null.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(true) with message TRAVEL_ADDED if saved.
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) if input is null, mapping fails, or file save fails—
        ///     state is fully restored on failure.
        ///   </description></item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool>> SaveTravel(Travel travel);

        /// <summary>
        /// Updates the Description of a specific <see cref="MemoryFile"/>.
        /// </summary>
        /// <param name="memory">
        /// The <see cref="MemoryFile"/> containing updated Description and valid FileID.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(true) if update succeeded.
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) if input is invalid, media not found, or save error.
        ///   </description></item>
        /// </list>
        /// </returns>
        ServiceResult<bool> UpdateMemory(MemoryFile memory);

        /// <summary>
        /// Asynchronously updates an existing <see cref="Travel"/>, including optional image replacement.
        /// Uses a database transaction to ensure atomicity;
        /// on any failure, the database is rolled back and any replaced file is restored.
        /// </summary>
        /// <param name="travel">
        /// The updated <see cref="Travel"/>. Must not be null and must reference an existing record.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/>:
        /// <list type="bullet">
        ///   <item><description>
        ///     Success(true) if update succeeds.
        ///   </description></item>
        ///   <item><description>
        ///     Failure(...) if input is invalid, mapping fails, or record not found—
        ///     state is fully restored on failure.
        ///   </description></item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool>> UpdateTravel(Travel travel);

        ServiceResult<byte[]> ExportTravel(int travelID);
        
        Task<ServiceResult<bool>> ImportTravel(byte[] travelFile);
    }
}
