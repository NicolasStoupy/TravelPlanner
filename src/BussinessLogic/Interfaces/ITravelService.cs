using BussinessLogic.Entities;
using BussinessLogic.Services.ServicesStatus;
using Commons;
using Commons.ErrorsHandlings;
using Commons.Models;
using System.Collections.ObjectModel;

namespace BussinessLogic.Interfaces
{
    public interface ITravelService
    {
        ServiceResult<Travel> GetTravel(int travelID);

        Task<ServiceResult<List<Travel>>> GetTravels(bool includeActivity = false, bool includeNotes = false, bool includeFollowers = false);

        Task<ServiceResult<bool>> AddMediaToTravel(List<byte[]> medias, int travelID, Commons.TypeMedia images);

        Task<ServiceResult<bool>> SaveTravel(Travel travel);

        Task<ServiceResult<bool>> DeleteTravel(int travelID);

        Task<ServiceResult<bool>> UpdateTravel(Travel travel);

        Task<ServiceResult<bool>> CloneTravel(Travel travel);

        ServiceResult<List<MemoryFile>> GetMemories(int id, TypeMedia mediaType);

        Task<ServiceResult<bool>> RemoveMemories(IEnumerable<MemoryFile>? selectedMemories, int id);

        ServiceResult<bool> UpdateMemory(MemoryFile memory);

      
    }
}