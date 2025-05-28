using AutoMapper;
using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Commons.Models;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BussinessLogic.Services
{
    public class LogBookService(
        IDbContextFactory<TravelPlannerContext> context,
        IMapper mapper,ILogger<LogBookService> logger) : ILogBookService
    {
        private readonly ILogger<LogBookService> _logger = logger;
        private readonly IDbContextFactory<TravelPlannerContext> _context = context;
        private readonly IMapper _mapper = mapper;


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
