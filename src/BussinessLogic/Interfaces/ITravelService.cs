using BussinessLogic.Entities;
using Commons.Models;


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
        Task<Result> AddNote(Note? note, int travelID);
        Task<Result> DeleteNote(Note note);
        Task<Result> EditNote(Note note);

        // Updating
    }
}