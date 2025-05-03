using AutoMapper;
using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Commons.Models;
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace BussinessLogic.Services
{
    public class TravelService(TravelPlannerContext context, IMapper mapper, DocumentProvider document) : ITravelService
    {
        private readonly TravelPlannerContext _context = context;
        private readonly DocumentProvider _document = document;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Retrieves a travel item by its ID and maps it from the Trip entity to a Travel domain model.
        /// </summary>
        /// <param name="travelID">The unique identifier of the travel item.</param>
        /// <returns>
        /// A <see cref="Travel"/> object mapped from the Trip entity if found;
        /// otherwise, a new empty <see cref="Travel"/> instance.
        /// </returns>
        public Travel GetTravel(int travelID)
        {
            var trip = _context.Trips.Where(t => t.TripId == travelID).FirstOrDefault();

            var Travel = _mapper.Map<Travel>(trip);

            return Travel ?? new Travel();
        }

        /// <summary>
        /// Retrieves all trips from the database, ordered by creation date,
        /// and maps them to a list of <see cref="Travel"/> domain models.
        /// </summary>
        /// <returns>
        /// A list of <see cref="Travel"/> items mapped from the database trips.
        /// </returns>
        public List<Travel> GetTravels(bool includeActivity = false, bool includeNotes = false, bool includeFollowers = false)
        {
            var trips = _context.Trips.OrderBy(t => t.CreatedAt).ToList();

            var travelItems = _mapper.Map<List<Travel>>(trips);

            return travelItems;
        }

        /// <summary>
        /// Deletes a travel record and all its related entities, including media, activities,
        /// logbooks, activity costs, and attendees.
        /// Ensures that all dependent data is cleaned up to maintain database integrity.
        /// </summary>
        /// <param name="travelID">The unique identifier of the travel record to delete.</param>
        /// <returns>
        /// A <see cref="Result"/> indicating whether the operation was successful.
        /// Returns a failure result if the trip does not exist or if an error occurs during deletion.
        /// </returns>
        public async Task<Result> DeleteTravel(int travelID)
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
                    .FirstOrDefaultAsync(t => t.TripId == travelID);

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
                await _context.SaveChangesAsync();
                return Result.Success("Le Voyage a été supprimé avec success");
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Saves a new travel record along with its optional background image.
        /// Performs the operation within a database transaction to ensure consistency between
        /// the database and the file system. If an error occurs, any saved image is removed to avoid orphan files.
        /// </summary>
        /// <param name="travel">The <see cref="Travel"/> domain object to be saved.</param>
        /// <returns>
        /// A <see cref="Result"/> indicating the success or failure of the operation.
        /// Returns a success message if the trip is saved correctly, or a failure message if an exception occurs.
        /// </returns>
        public async Task<Result> SaveTravel(Travel travel)
        {
            var trip = _mapper.Map<Trip>(travel);
            trip.CurrencyCode = travel.currencie;
            Guid? savedFileGuid = null;
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Save The trip without image
                _context.Trips.Add(trip);
                await _context.SaveChangesAsync();

                // 2. Save Image
                if (travel.image != null)
                {
                    _document.SetMediaType(Commons.TypeMedia.Images);
                    var savedImageGuid = _document.SaveFile(travel.image);

                    // 3. Update the trip information
                    trip.TripBackgroundGuid = savedImageGuid;
                    _context.Trips.Update(trip);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return Result.Success($"Le voyage {travel.name} a bien été créé !");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                //  remove orphan file
                if (savedFileGuid.HasValue)
                {
                    _document.RemoveFile(savedFileGuid.Value, Commons.TypeMedia.Images);
                }
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> UpdateTravel(Travel travel)
        {
            var trip = _mapper.Map<Trip>(travel);


            Guid? savedFileGuid = null;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingTrip = await _context.Trips.FindAsync(trip.TripId);
                if (existingTrip == null)
                {
                    return Result.Failure($"Le voyage avec l'ID {trip.TripId} est introuvable.");
                }

                // 1. Update les propriétés (hors image pour le moment)
                _context.Entry(existingTrip).CurrentValues.SetValues(trip);

                await _context.SaveChangesAsync();

                // 2. Save nouvelle image si nécessaire
                if (travel.image != null)
                {
                    _document.SetMediaType(Commons.TypeMedia.Images);
                    savedFileGuid = _document.ReplaceFile(travel.imageID, travel.image);

                    // 3. Mise à jour du GUID image
                    existingTrip.TripBackgroundGuid = savedFileGuid;
                    _context.Trips.Update(existingTrip);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return Result.Success($"Le voyage {travel.name} a bien été mis à jour !");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Suppression de l'image si elle avait été enregistrée
                if (savedFileGuid.HasValue)
                {
                    _document.RemoveFile(savedFileGuid.Value, Commons.TypeMedia.Images);
                }

                return Result.Failure($"Une erreur est survenue lors de la mise à jour : {ex.Message}");
            }
        }

        public Task<Result> AddNote(Note? note, int travelID)
        {
            var log = _mapper.Map<LogBook>(note);
            var trip = _context.Trips.FirstOrDefault(t => t.TripId == travelID);
            if (trip != null)
            {

                trip.LogBooks.Add(log);
                _context.SaveChanges();

                return Task.FromResult(Result.Success("Note Ajoutée aevc success"));
            }
            else
            {
                return Task.FromResult(Result.Failure("Le Voyage n'existe pas "));
            }
        }

        public Task<Result> DeleteNote(Note note)
        {
            try
            {
                var log = _context.LogBooks.FirstOrDefault(l => l.LogBookId == note.NoteId);
                if(log == null) return Task.FromResult(Result.Failure("Note not found"));
                _context.LogBooks.Remove(log);
                _context.SaveChanges();
                return Task.FromResult(Result.Success("Supprimé"));
            }
            catch (Exception ex )
            {

                return Task.FromResult( Result.Failure(ex.Message));
            }


        }

        public Task<Result> EditNote(Note note)
        {
            try
            {
                var log = _context.LogBooks.FirstOrDefault(l => l.LogBookId == note.NoteId);
                if (log == null) return Task.FromResult(Result.Failure("Note not found"));
                log.Description = note.NoteContent;
                _context.LogBooks.Update(log);
                _context.SaveChanges();
                return Task.FromResult(Result.Success("Updated"));
            }
            catch (Exception ex)
            {

                return Task.FromResult(Result.Failure(ex.Message));
            }
        }
    }
}