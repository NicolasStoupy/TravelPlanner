using Infrastructure.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Interfaces
{
    public interface IMediaService
    {

        List<byte[]> GetMediasFromTrip(Trip trip, List<MediaType> mediaTypes);
        List<byte[]> GetMediasFromActivity(Activity activity, List<MediaType> mediaTypes);
        List<byte[]> GetMediasFromCosting(ActivityCost activity, List<MediaType> mediaTypes);
        byte[]? GetMedia(Guid fileGuid);
        Guid? SaveMedia(byte[] fileBytes, MediaType mediaType);

    }
}
