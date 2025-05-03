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

        public List<byte[]> GetMediasFromTrip(Trip trip, List<MediaType> mediaTypes)
        {
            var result = new List<byte[]>();
     

            return result;
        }


        public byte[]? GetMedia(Guid fileGuid, TypeMedia typeMedia) { return _document.GetFile(fileGuid,typeMedia); }

        public Guid? SaveMedia(byte[]? fileBytes, TypeMedia typeMedia)
        {
            if (fileBytes == null) return null;
            _document.SetMediaType(typeMedia);
            return _document.SaveFile(fileBytes);
        }
        private Guid ConvertToGuid(string id)
        {
            Guid fileGuid;
            Guid.TryParse(id, out fileGuid);
            return fileGuid;
        }

    
    }
}
