
using AutoMapper;
using BussinessLogic.DTOs;
using BussinessLogic.Interfaces;
using Infrastructure.EntityModels;


namespace BussinessLogic.Services
{
    public class TripService : ITripService
    {
        private readonly TravelPlannerContext _context;
        private readonly IMapper _mapper;

        public TripService(TravelPlannerContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        async Task<ExecutionStatus> ITripService.CreateTrip(TripDTO newTrip)
        {
            try
            {
                Trip tripEntity = _mapper.Map<Trip>(newTrip);
                _context.Trips.Add(tripEntity);
                await _context.SaveChangesAsync();
                return ExecutionStatus.Success;
            }
            catch
            {
                return ExecutionStatus.Failure;

            }

        }

        List<TripDTO> ITripService.GetTrips()
        {
            List<TripDTO> tripList = _mapper.Map<List<TripDTO>>(_context.Trips.ToList());

            return tripList;
        }
    }
}
