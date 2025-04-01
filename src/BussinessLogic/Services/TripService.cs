using AutoMapper;
using BussinessLogic.DTOs;
using BussinessLogic.Interfaces;
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using Microsoft.CodeAnalysis;


namespace BussinessLogic.Services
{
    public class TripService : ITripService
    {
        private readonly TravelPlannerContext _context;
        private readonly DocumentProvider _document;
        private readonly IMapper _mapper;

        public TripService(TravelPlannerContext context, IMapper mapper, DocumentProvider document)
        {
            _context = context;
            _mapper = mapper;
            _document = document;
        }

        public List<byte[]> GetTripsMedia(MediaType mediaType, int tripId)
        {
            var result = new List<byte[]>();
            try
            {
                var trip = _context.Trips.FirstOrDefault(t => t.TripId == tripId);

                if (trip != null)
                    foreach (var media in trip.Media)
                    {
                        Guid guid;
                        if (Guid.TryParse(media.MediaID, out guid))
                        {
                            var fileByte = _document.GetFile(guid);

                            if (fileByte != null)
                                result.Add(fileByte);
                        }
                    }

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
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
