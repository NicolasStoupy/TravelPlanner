using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using BussinessLogic.Models;
using Commons;
using Commons.ErrorsHandlings;
using Commons.Resources;
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

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

        public async Task<ServiceResult<string>> ExportMemoriesToZip(
            IEnumerable<MemoryFile> memoryFiles,
            TypeMedia mediaType,
            string zipPath,
            string fileName)
        {
            if (memoryFiles == null || !memoryFiles.Any())
                return ServiceResult<string>.Failure(MediaServiceMessages.INVALID_INPUT);

            var guids = memoryFiles.Select(m => m.FileGuid);
            var path = await _document.ExportToZipAsync(guids, mediaType, zipPath, fileName);

            return ServiceResult<string>.Success(path);
        }

      

        public ServiceResult<byte[]> GetMedia(Guid fileGuid, TypeMedia typeMedia)
        {
            var file = _document.GetFile(fileGuid, typeMedia);
            if (file == null)
                return ServiceResult<byte[]>.Failure(MediaServiceMessages.NOT_FOUND);

            return ServiceResult<byte[]>.Success(file);
        }

        public List<byte[]> GetMediasFromTrip(int tripID, TypeMedia mediaTypes)
        {
            using var context = _context.CreateDbContext();
            var trip = context.Trips.Include(t => t.Media).FirstOrDefault(t => t.TripId == tripID);

            if (trip != null)
            {
                var medias = GetMediasFromTrip(trip, mediaTypes);
                if (medias.IsSuccess)
                {
                    return medias.Value;
                }
            }
            return new List<byte[]>();
        }

        public ServiceResult<List<Guid>> SaveMedias(List<byte[]> files, TypeMedia typeMedia)
        {
            if (files == null || files.Count == 0)
                return ServiceResult<List<Guid>>.Failure(MediaServiceMessages.INVALID_INPUT);

            var guids = new List<Guid>();
            foreach (var file in files)
            {
                var res = SaveMedia(file, typeMedia);
                if (!res.IsSuccess)
                {
                    _document.RemoveFiles(guids, typeMedia);
                    return ServiceResult<List<Guid>>.Failure(res.Message);
                }
                guids.Add(res.Value);
            }
            return ServiceResult<List<Guid>>.Success(guids);
        }

        private ServiceResult<List<byte[]>> GetMediasFromTrip(Trip trip, TypeMedia typeMedia)
        {
            if (trip == null)
                return ServiceResult<List<byte[]>>.Failure(TravelServiceMessage.INVALID_TRAVEL);

            var result = new List<byte[]>();
            foreach (var media in trip.Media)
            {
                var file = _document.GetFile(media.FileGuid, typeMedia);
                if (file != null) result.Add(file);
            }
            return ServiceResult<List<byte[]>>.Success(result);
        }
        private ServiceResult<Guid> SaveMedia(byte[] fileBytes, TypeMedia typeMedia)
        {
            if (fileBytes == null || fileBytes.Length == 0)
                return ServiceResult<Guid>.Failure(MediaServiceMessages.INVALID_INPUT);

            _document.SetMediaType(typeMedia);
            var guid = _document.SaveFile(fileBytes);
            if (guid == null)
                return ServiceResult<Guid>.Failure(MediaServiceMessages.NOT_FOUND);

            return ServiceResult<Guid>.Success(guid.Value);
        }
    }
}