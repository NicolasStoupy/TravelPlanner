using BussinessLogic.Interfaces;
using Commons;
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace BussinessLogic.Services
{
    public class MediaService : IMediaService
    {
        private readonly IDbContextFactory<TravelPlannerContext> _context;
        private readonly DocumentProvider _document;

        public MediaService(IDbContextFactory<TravelPlannerContext> context, DocumentProvider documentProvider)
        {
            _context = context;
            _document = documentProvider;
        }

        public List<byte[]> GetMediasFromActivity(Activity activity, List<MediaType> mediaTypes)
        {
            throw new NotImplementedException();
        }

        public List<byte[]> GetMediasFromCosting(ActivityCost activity, List<MediaType> mediaTypes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves media files associated with the specified trip and filtered by given media types.
        /// </summary>
        /// <param name="trip">The trip to retrieve media files for.</param>
        /// <param name="mediaTypes">A list of media types to filter the results (e.g., images, videos).</param>
        /// <returns>A list of byte arrays representing the media files.</returns>
        public List<byte[]> GetMediasFromTrip(Trip trip, List<MediaType> mediaTypes)
        {
            var result = new List<byte[]>();

            return result;
        }

        public byte[]? GetMedia(Guid fileGuid, TypeMedia typeMedia)
        { return _document.GetFile(fileGuid, typeMedia); }

        /// <summary>
        /// Saves a media file to storage and returns its unique identifier.
        /// </summary>
        /// <param name="fileBytes">The binary content of the media file to save.</param>
        /// <param name="typeMedia">The type of the media (e.g., image, video, document).</param>
        /// <returns>
        /// A <see cref="Guid"/> representing the saved file's identifier, or <c>null</c> if <paramref name="fileBytes"/> is null.
        /// </returns>
        public Guid? SaveMedia(byte[]? fileBytes, TypeMedia typeMedia)
        {
            if (fileBytes == null) return null;
            _document.SetMediaType(typeMedia);
            return _document.SaveFile(fileBytes);
        }
    }
}