using AutoMapper;
using BussinessLogic.Entities;
using BussinessLogic.Extensions;
using BussinessLogic.Interfaces;
using Commons.Models;
using Commons.Resources;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BussinessLogic.Services
{
    /// <summary>
    /// Provides operations for adding, deleting, and editing logbook entries (notes)
    /// associated with a travel record.  
    /// All methods return a <see cref="ServiceResult{T}"/>, and on failure 
    /// no changes are persisted (the initial state is restored).
    /// </summary>
    public class LogBookService(
        IDbContextFactory<TravelPlannerContext> context,
        IMapper mapper, ILogger<LogBookService> logger) : ILogBookService
    {
        private readonly ILogger<LogBookService> _logger = logger;
        private readonly IDbContextFactory<TravelPlannerContext> _context = context;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Adds a new note to the specified travel.
        /// If the travel is not found or <paramref name="note"/> is null,
        /// no state change occurs.
        /// </summary>
        /// <param name="note">The note to add; may be null.</param>
        /// <param name="travelID">The ID of the travel to attach the note to.</param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       Success(true) if the note was added.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       Failure(...) if the input is invalid or the travel was not found;
        ///       no changes are saved.
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        public async Task<ServiceResult<bool>> AddNoteAsync(Note? note, int travelID)
        {
            if (note == null)
                return ServiceResult<bool>.Failure(LogBookServiceMessages.INVALID_NOTE);

            await using var ctx = _context.CreateDbContext();
            var trip = await ctx.Trips.FindAsync(travelID);
            if (trip == null)
                return ServiceResult<bool>.Failure(LogBookServiceMessages.TRAVEL_NOT_FOUND);

            try
            {
                var logEntity = _mapper.Map<LogBook>(note);
                trip.LogBooks.Add(logEntity);
                await ctx.SaveChangesAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error adding note to travel {TravelID}", travelID);
                return ServiceResult<bool>.Failure(LogBookServiceMessages.DATABASE_ERROR);
            }
        }

        public async Task<ServiceResult<bool>> AddNoteToActivityAsync(Note? note, int? activityID, int? travelID)
        {
            if (activityID <= 0)
                return ServiceResult<bool>.Failure(LogBookServiceMessages.INVALID_NOTE);

            await using var ctx = _context.CreateDbContext();
            var activity = await ctx.Activities.FindAsync(travelID, activityID);
            if (activity == null)
                return ServiceResult<bool>.Failure(LogBookServiceMessages.ACTIVITY_NOT_FOUND);

            try
            {
                var logEntity = _mapper.Map<LogBook>(note);
                activity.LogBooks.Add(logEntity);
                await ctx.SaveChangesAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error adding note to Activity {activityID}", activityID);
                return ServiceResult<bool>.Failure(LogBookServiceMessages.DATABASE_ERROR);
            }

        }

        /// <summary>
        /// Deletes an existing note.
        /// If the note is not found, no state change occurs.
        /// </summary>
        /// <param name="note">The note to delete (identifies the log entry).</param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       Success(true) if the note was deleted.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       Failure(...) if the note was not found or a database error occurred;
        ///       no partial deletions occur.
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        public async Task<ServiceResult<bool>> DeleteNoteAsync(Note note)
        {
            if (note == null || note.NoteId <= 0)
                return ServiceResult<bool>.Failure(LogBookServiceMessages.INVALID_NOTE);

            await using var ctx = _context.CreateDbContext();
            var entity = await ctx.LogBooks.FindAsync(note.NoteId);
            if (entity == null)
                return ServiceResult<bool>.Failure(LogBookServiceMessages.NOTE_NOT_FOUND);

            try
            {
                ctx.LogBooks.Remove(entity);
                await ctx.SaveChangesAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error deleting note {NoteId}", note.NoteId);
                return ServiceResult<bool>.Failure(LogBookServiceMessages.DATABASE_ERROR);
            }

        }
        /// <summary>
        /// Updates the content of an existing note.
        /// If the note is not found, no state change occurs.
        /// </summary>
        /// <param name="note">The note with updated content and a valid NoteId.</param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/>:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       Success(true) if the note was updated.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       Failure(...) if the note was not found or a database error occurred;
        ///       no partial updates occur.
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        public async Task<ServiceResult<bool>> EditNoteAsync(Note note)
        {
            if (note == null || note.NoteId <= 0)
                return ServiceResult<bool>.Failure(LogBookServiceMessages.INVALID_NOTE);

            await using var ctx = _context.CreateDbContext();
            var entity = await ctx.LogBooks.FindAsync(note.NoteId);
            if (entity == null)
                return ServiceResult<bool>.Failure(LogBookServiceMessages.NOTE_NOT_FOUND);

            try
            {
                entity.Description = note.NoteContent ?? string.Empty;
                ctx.LogBooks.Update(entity);
                await ctx.SaveChangesAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error updating note {NoteId}", note.NoteId);
                return ServiceResult<bool>.Failure(LogBookServiceMessages.DATABASE_ERROR);
            }

        }

        public async Task<ServiceResult<List<Note>>> GetActivityNotes(int? activityID)
        {
            //Vérifier que l’ID est valide
            if (activityID == null || activityID <= 0)
            {
                return ServiceResult<List<Note>>
                    .Failure(ActivityServiceMessage.ActivityServiceStatus_InvalidActivity_Message);
            }

            //Charger en base tous les LogBooks (notes) liés à cette activité
            await using var ctx = _context.CreateDbContext();
            var logBooks = await ctx.LogBooks
                                     .Where(l => l.ActivityId == activityID)
                                     .ToListAsync();

            //Si aucune note associée n’a été trouvée, renvoyer un échec
            if (logBooks == null )
            {
                return ServiceResult<List<Note>>
                    .Failure(LogBookServiceMessages.NOTE_NOT_FOUND);
            }

            //Mapper les entités LogBook vers vos objets métier Note
            if (!_mapper.TryMap(logBooks, out List<Note> notes, _logger))
            {
                return ServiceResult<List<Note>>
                    .Warning(GlobalServiceMessage.UNKNOWN_ERROR);
            }

            //Retourner la liste mappée en succès
            return ServiceResult<List<Note>>.Success(notes);
        }
    }
}
