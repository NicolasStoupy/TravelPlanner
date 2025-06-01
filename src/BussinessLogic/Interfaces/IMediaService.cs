using BussinessLogic.Entities;
using Commons;
using Commons.Models;
using Infrastructure.EntityModels;

namespace BussinessLogic.Interfaces
{
    public interface IMediaService
    {
        /// <summary>
        /// Exports the specified memory files to a ZIP archive.
        /// </summary>
        /// <param name="memoryFiles">The collection of memory files to include in the ZIP.</param>
        /// <param name="mediaType">The media type of the files to export.</param>
        /// <param name="zipPath">The directory path where the ZIP will be created.</param>
        /// <param name="fileName">The desired name of the resulting ZIP file.</param>
        /// <returns>
        /// A <see cref="Task{ServiceResult{String}}"/> which, when completed, contains the full path
        /// to the created ZIP on success, or a failure result with an error message.
        /// No modifications to stored media occur on failure.
        /// </returns>
        Task<ServiceResult<string>> ExportMemoriesToZip(
            IEnumerable<MemoryFile> memoryFiles,
            TypeMedia mediaType,
            string zipPath,
            string fileName);

       

        /// <summary>
        /// Retrieves a single media file by its unique identifier and type.
        /// </summary>
        /// <param name="fileGuid">The GUID of the media file to retrieve.</param>
        /// <param name="typeMedia">The type of media (e.g., image, video, document).</param>
        /// <returns>
        /// A <see cref="ServiceResult{Byte[]}"/> containing the file bytes on success,
        /// or a failure result if the file is not found or an error occurs.  
        /// No state changes occur on failure.
        /// </returns>
        ServiceResult<byte[]> GetMedia(Guid fileGuid, TypeMedia typeMedia);

        /// <summary>
        /// Saves multiple media files of the specified type and returns their identifiers.
        /// If any save operation fails, all previously saved files in this batch are removed
        /// and the system state is restored to its initial condition.
        /// </summary>
        /// <param name="files">A list of byte arrays representing the media files to save.</param>
        /// <param name="typeMedia">The type of media for all files in the list.</param>
        /// <returns>
        /// A <see cref="ServiceResult{List{Guid}}"/> containing the list of saved file GUIDs on success,
        /// or a failure result with an error message if any file could not be saved,
        /// in which case no partial data remains.
        /// </returns>
        ServiceResult<List<Guid>> SaveMedias(List<byte[]> files, TypeMedia typeMedia);
    }
}
