using BussinessLogic.Entities;
using Commons;
using Infrastructure.EntityModels;
using System.Collections.ObjectModel;

namespace BussinessLogic.Interfaces
{
    public interface IMediaService
    {
        List<byte[]> GetMediasFromTrip(int tripID, TypeMedia typeMedia);
       
        List<byte[]> GetMediasFromTrip(Trip trip, List<MediaType> mediaTypes);
        List<byte[]> GetMediasFromActivity(Activity activity, TypeMedia typeMedia);
        List<byte[]> GetMediasFromCosting(ActivityCost activity, List<MediaType> mediaTypes);
        byte[]? GetMedia(Guid fileGuid,TypeMedia typeMedia  );
        Guid? SaveMedia(byte[]? fileBytes, TypeMedia typeMedia);
        List<Guid?> SaveMedias(List<byte[]> files, TypeMedia images);
        Task<string> ExportMemoriesToZip(IEnumerable<MemoryFile> memoryFiles, TypeMedia mediaType, string zipPath, string fileName);
    }
}
