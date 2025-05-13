using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Commons;
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
        public List<byte[]> GetMediasFromTrip(Trip trip, TypeMedia typeMedia)
        {
            var result = new List<byte[]>();
            using var context = _context.CreateDbContext();
            var medias = trip.Media.ToList();

            foreach (var media in medias)
            {
                var file = _document.GetFile(media.FileGuid, typeMedia);
                if (file != null)
                {
                    result.Add(file);
                }
            }

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

        public List<byte[]> GetMediasFromTrip(int tripID, TypeMedia mediaTypes)
        {
            using var context = _context.CreateDbContext();
            var trip = context.Trips.Include(t => t.Media).FirstOrDefault(t => t.TripId == tripID);

            if (trip != null)
            {
                return GetMediasFromTrip(trip, mediaTypes);
            }
            return new List<byte[]>();
        }

        public List<byte[]> GetMediasFromTrip(Trip trip, List<MediaType> mediaTypes)
        {
            throw new NotImplementedException();
        }

        public List<byte[]> GetMediasFromActivity(Activity activity, TypeMedia typeMedia)
        {
            throw new NotImplementedException();
        }

        public List<Guid?> SaveMedias(List<byte[]> files, TypeMedia images)
        {
            var result = new List<Guid?>();
            foreach (var file in files)
            {
                var savedMediaGuid = SaveMedia(file, images);
                if (savedMediaGuid != null)
                    result.Add(savedMediaGuid);
            }
            return result;
        }

        public async Task<string> ExportMemoriesToZip(IEnumerable<MemoryFile> memoryFiles, TypeMedia mediaType, string zipPath, string fileName)
        {
            var result = string.Empty;
            var guidList = memoryFiles.Select(mf => mf.FileGuid);
            if (guidList.Any())
            {
                result = await _document.ExportToZipAsync(guidList, mediaType, zipPath, fileName);
            }

            return result.ToString();
        }
       
    }
}