using AutoMapper;
using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using BussinessLogic.Models;
using Infrastructure.Documents;
using Infrastructure.EntityModels;



namespace BussinessLogic.Services
{
    public class TravelService : ITravelService
    {
        private readonly TravelPlannerContext _context;
        private readonly DocumentProvider _document;
        private readonly IMapper _mapper;

        public TravelService(TravelPlannerContext context, IMapper mapper, DocumentProvider document)
        {
            _context = context;
            _mapper = mapper;
            _document = document;
        }

        public List<TravelItem> GetTravelItems()
        {
            List<TravelItem> travelItems = new List<TravelItem>();
            var trips = _context.Trips.OrderBy(t => t.CreatedAt);
            _document.SetMediaType(MediaType.Images.ToString());
            foreach (var trip in trips)
            {
                travelItems.Add(new TravelItem
                {
                    Id = trip.TripId,
                    description = trip.Description,
                    EndDate = trip.EndDate,
                    name = trip.Name,
                    StartDate = trip.StartDate,
                    travelDate = trip.StartDate,
                    image = trip.TripBackgroundGuid.HasValue ? _document.GetFile(trip.TripBackgroundGuid.Value) : null

                });

            }

            return travelItems;
        }
    
        public async Task<Result> SaveTrip(Trip trip, byte[]? image)
        {

            try
            {
                _context.Trips.Add(trip);
                await _context.SaveChangesAsync();
                // Save document only if adding success
                trip.TripBackgroundGuid = _document.SaveFile(image);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                return Result.Failure(ex.Message);
            }

            return Result.Success();
        }
    }
}
