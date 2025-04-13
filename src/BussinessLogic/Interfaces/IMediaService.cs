using Commons;
using Infrastructure.EntityModels;

namespace BussinessLogic.Interfaces
{
    public interface IMediaService
    {

        List<byte[]> GetMediasFromTrip(Trip trip, List<MediaType> mediaTypes);
        List<byte[]> GetMediasFromActivity(Activity activity, List<MediaType> mediaTypes);
        List<byte[]> GetMediasFromCosting(ActivityCost activity, List<MediaType> mediaTypes);
        byte[]? GetMedia(Guid fileGuid,TypeMedia typeMedia  );
        Guid? SaveMedia(byte[]? fileBytes, TypeMedia typeMedia);

    
      
    }
}
