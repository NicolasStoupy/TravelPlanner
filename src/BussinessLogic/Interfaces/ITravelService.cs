using BussinessLogic.Entities;
using Commons.Models;
using Infrastructure.EntityModels;

namespace BussinessLogic.Interfaces
{
    public interface ITravelService
    {
        // Reading
        Travel GetTravel(int tripID);

        List<Travel> GetTravels(bool includeActivity = false, bool includeNotes = false, bool includeFollowers = false);

        // Writting
        Task<Result> DeleteTravel(int travelID);

        Task<Result> SaveTravel(Travel travel);
        Task<Result> UpdateTravel(Travel travel);

        // Updating
    }
}