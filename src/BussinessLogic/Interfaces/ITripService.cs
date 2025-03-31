using BussinessLogic.DTOs;

namespace BussinessLogic.Interfaces
{


    public interface ITripService
    {
        Task<ExecutionStatus> CreateTrip(TripDTO newTrip);
        List<TripDTO> GetTrips();
    }
}
