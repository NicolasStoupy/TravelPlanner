using AutoMapper;
using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using BussinessLogic.Models;
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;



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

        public async Task<Result> DeleteTrip(int tripId)
        {
            try
            {
                var trip = await _context.Trips
                    .Include(t => t.LogBooks)
                    .Include(t => t.Media)
                    .Include(t => t.Activities)
                    .ThenInclude(a => a.LogBooks)
                    .Include(t => t.Activities)
                    .ThenInclude(a => a.ActivityCosts)
                   .Include(t => t.Activities).ThenInclude(a => a.Attendees)
                    .FirstOrDefaultAsync(t => t.TripId == tripId);

                if (trip == null)
                {

                    return Result.Failure("Trip not exist");

                }
                _context.RemoveRange(trip.Media);

                foreach (var activity in trip.Activities)
                {
                    // Supprimer les LogBooks des Activity
                    _context.LogBooks.RemoveRange(activity.LogBooks);


                    foreach (var activityCost in activity.ActivityCosts)
                    {
                        // Supprimer les Media des ActivityCost
                        _context.Media.RemoveRange(activityCost.Media);
                    }

                    // Supprimer les Attendees 
                    _context.Attendees.RemoveRange(activity.Attendees);

                    // Supprimer les ActivityCosts
                    _context.ActivityCosts.RemoveRange(activity.ActivityCosts);
                    _context.Remove(activity);
                }

                _context.Remove(trip);
                _context.SaveChanges();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public List<TravelItem> GetTravelItems()
        {
            List<TravelItem> travelItems = new List<TravelItem>();
            var trips = _context.Trips.OrderBy(t => t.CreatedAt);
            //_document.SetMediaType(MediaType.Images.ToString());
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
                if (image != null)
                {
                    // Save document only if adding success
                    _document.SetMediaType(Commons.TypeMedia.Images);
                    trip.TripBackgroundGuid = _document.SaveFile(image);
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {

                return Result.Failure(ex.Message);
            }

            return Result.Success();
        }
    }
}
