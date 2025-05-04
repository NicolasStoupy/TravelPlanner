using AutoMapper;
using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Commons.Models;
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace BussinessLogic.Services
{
    /// <summary>
    /// Provides business logic for managing travel data, including creation, update, deletion,
    /// note management, and media handling. This service abstracts access to the underlying database
    /// and file system using Entity Framework Core and AutoMapper.
    /// </summary>
    /// <remarks>
    /// This service uses <see cref="IDbContextFactory{TContext}"/> to manage database contexts in a scoped and thread-safe way,
    /// and maps between infrastructure entities and domain models using <see cref="IMapper"/>.
    /// Media operations such as file storage and replacement are handled via the <see cref="DocumentProvider"/>.
    /// </remarks>
    public class TravelService(IDbContextFactory<TravelPlannerContext> context, IMapper mapper, DocumentProvider document) : ITravelService
    {
        private readonly IDbContextFactory<TravelPlannerContext> _context = context;
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
            using var context = _context.CreateDbContext();
            var trip = context.Trips.Where(t => t.TripId == travelID).FirstOrDefault();

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
            using var context = _context.CreateDbContext();
            var trips = context.Trips.OrderBy(t => t.CreatedAt).ToList();

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
            using var context = _context.CreateDbContext();
            try
            {
                var trip = await context.Trips
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
                context.RemoveRange(trip.Media);

                foreach (var activity in trip.Activities)
                {
                    // Supprimer les LogBooks des Activity
                    context.LogBooks.RemoveRange(activity.LogBooks);

                    foreach (var activityCost in activity.ActivityCosts)
                    {
                        // Supprimer les Media des ActivityCost
                        context.Media.RemoveRange(activityCost.Media);
                    }

                    // Supprimer les Attendees
                    context.Attendees.RemoveRange(activity.Attendees);

                    // Supprimer les ActivityCosts
                    context.ActivityCosts.RemoveRange(activity.ActivityCosts);
                    context.Remove(activity);
                }

                context.Remove(trip);
                await context.SaveChangesAsync();
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
            using var context = _context.CreateDbContext();
            var trip = _mapper.Map<Trip>(travel);
            trip.CurrencyCode = travel.currencie;
            Guid? savedFileGuid = null;
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // 1. Save The trip without image
                context.Trips.Add(trip);
                await context.SaveChangesAsync();

                // 2. Save Image
                if (travel.image != null)
                {
                    _document.SetMediaType(Commons.TypeMedia.Images);
                    var savedImageGuid = _document.SaveFile(travel.image);

                    // 3. Update the trip information
                    trip.TripBackgroundGuid = savedImageGuid;
                    context.Trips.Update(trip);
                    await context.SaveChangesAsync();
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

        /// <summary>
        /// Updates an existing travel entry, including replacing its associated image if provided.
        /// </summary>
        /// <param name="travel">The updated travel data, including the optional new image.</param>
        /// <returns>
        /// A <see cref="Result"/> indicating whether the update was successful or failed, with a message.
        /// </returns>
        public async Task<Result> UpdateTravel(Travel travel)
        {
            using var context = _context.CreateDbContext();
            var trip = _mapper.Map<Trip>(travel);

            Guid? savedFileGuid = null;

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var existingTrip = await context.Trips.FindAsync(trip.TripId);
                if (existingTrip == null)
                {
                    return Result.Failure($"Le voyage avec l'ID {trip.TripId} est introuvable.");
                }

                // 1. Update les propriétés (hors image pour le moment)
                context.Entry(existingTrip).CurrentValues.SetValues(trip);

                await context.SaveChangesAsync();

                // 2. Save nouvelle image si nécessaire
                if (travel.image != null)
                {
                    _document.SetMediaType(Commons.TypeMedia.Images);
                    savedFileGuid = _document.ReplaceFile(travel.imageID, travel.image);

                    // 3. Mise à jour du GUID image
                    existingTrip.TripBackgroundGuid = savedFileGuid;
                    context.Trips.Update(existingTrip);
                    await context.SaveChangesAsync();
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

        /// <summary>
        /// Adds a new note to a specific travel (trip) identified by its ID.
        /// </summary>
        /// <param name="note">The note to add. Can be <c>null</c>.</param>
        /// <param name="travelID">The ID of the travel to which the note should be added.</param>
        /// <returns>
        /// A <see cref="Result"/> indicating whether the operation succeeded or failed, wrapped in a <see cref="Task"/>.
        /// </returns>
        public Task<Result> AddNote(Note? note, int travelID)
        {
            using var context = _context.CreateDbContext();
            var log = _mapper.Map<LogBook>(note);
            var trip = context.Trips.FirstOrDefault(t => t.TripId == travelID);
            if (trip != null)
            {
                trip.LogBooks.Add(log);
                context.SaveChanges();

                return Task.FromResult(Result.Success("Note Ajoutée aevc success"));
            }
            else
            {
                return Task.FromResult(Result.Failure("Le Voyage n'existe pas "));
            }
        }

        /// <summary>
        /// Deletes a note from the database based on its identifier.
        /// </summary>
        /// <param name="note">The note to delete.</param>
        /// <returns>
        /// A <see cref="Result"/> wrapped in a <see cref="Task"/>, indicating whether the deletion was successful or failed.
        /// </returns>
        public Task<Result> DeleteNote(Note note)
        {
            using var context = _context.CreateDbContext();
            try
            {
                var log = context.LogBooks.FirstOrDefault(l => l.LogBookId == note.NoteId);
                if (log == null) return Task.FromResult(Result.Failure("Note not found"));
                context.LogBooks.Remove(log);
                context.SaveChanges();
                return Task.FromResult(Result.Success("Supprimé"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result.Failure(ex.Message));
            }
        }

        /// <summary>
        /// Updates the content of an existing note.
        /// </summary>
        /// <param name="note">The note containing the updated content and identifier.</param>
        /// <returns>
        /// A <see cref="Result"/> wrapped in a <see cref="Task"/>, indicating whether the update was successful or failed.
        /// </returns>
        public Task<Result> EditNote(Note note)
        {
            using var context = _context.CreateDbContext();
            try
            {
                var log = context.LogBooks.FirstOrDefault(l => l.LogBookId == note.NoteId);
                if (log == null) return Task.FromResult(Result.Failure("Note not found"));
                log.Description = note.NoteContent;
                context.LogBooks.Update(log);
                context.SaveChanges();
                return Task.FromResult(Result.Success("Updated"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result.Failure(ex.Message));
            }
        }
    }
}