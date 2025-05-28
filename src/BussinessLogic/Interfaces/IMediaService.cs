using BussinessLogic.Entities;
using Commons;

namespace BussinessLogic.Interfaces
{
    public interface IMediaService
    {
        byte[] GeneratePdfSummary(Travel travelID);
        List<byte[]> GetMediasFromTrip(int tripID, TypeMedia typeMedia);     
        byte[]? GetMedia(Guid fileGuid,TypeMedia typeMedia  );
        Guid? SaveMedia(byte[]? fileBytes, TypeMedia typeMedia);
        List<Guid> SaveMedias(List<byte[]> files, TypeMedia images);        
        Task<string> ExportMemoriesToZip(IEnumerable<MemoryFile> memoryFiles, TypeMedia mediaType, string zipPath, string fileName);
      
    }
}
