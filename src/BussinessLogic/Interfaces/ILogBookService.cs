using BussinessLogic.Entities;
using Commons.ErrorsHandlings;
using System.Collections.ObjectModel;

namespace BussinessLogic.Interfaces
{
    /// <summary>
    /// Provides operations for adding, deleting, and editing logbook entries (notes)
    /// associated with a travel record.
    /// All methods return a <see cref="ServiceResult{T}"/>, and on failure
    /// any partial changes are rolled back so that the system state is unchanged.
    /// </summary>
    public interface ILogBookService
    {
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
        ///       <see cref="ServiceResult{Boolean}.Success"/> with <c>true</c> if the note was added.
        ///   </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="LogBookServiceMessages.INVALID_NOTE"/> if the note is null,
        ///       or <see cref="LogBookServiceMessages.TRAVEL_NOT_FOUND"/> if the travel does not exist.
        ///   </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="LogBookServiceMessages.DATABASE_ERROR"/> on a database exception.
        ///   </description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool>> AddNoteAsync(Note? note, int travelID);

        Task<ServiceResult<bool>> AddNoteToActivityAsync(Note? note, int? activityID,int? travelID);

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
        ///       <see cref="ServiceResult{Boolean}.Success"/> with <c>true</c> if the note was deleted.
        ///   </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="LogBookServiceMessages.INVALID_NOTE"/> if the note is null or invalid,
        ///       or <see cref="LogBookServiceMessages.NOTE_NOT_FOUND"/> if not found.
        ///   </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="LogBookServiceMessages.DATABASE_ERROR"/> on a database exception.
        ///   </description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool>> DeleteNoteAsync(Note note);

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
        ///       <see cref="ServiceResult{Boolean}.Success"/> with <c>true</c> if the note was updated.
        ///   </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="LogBookServiceMessages.INVALID_NOTE"/> if the note is null or invalid,
        ///       or <see cref="LogBookServiceMessages.NOTE_NOT_FOUND"/> if not found.
        ///   </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="LogBookServiceMessages.DATABASE_ERROR"/> on a database exception.
        ///   </description>
        ///   </item>
        /// </list>
        /// </returns>
        Task<ServiceResult<bool>> EditNoteAsync(Note note);
        Task<ServiceResult<List<Note>>> GetActivityNotes(int? activityID);
    }

}