namespace BussinessLogic.Entities
{
    /// <summary>
    /// Represents a textual note associated with a travel activity or memory.
    /// </summary>
    public class Note
    {
        /// <summary>
        /// Gets or sets the unique identifier for the note.
        /// </summary>
        public int NoteId { get; set; }

        /// <summary>
        /// Gets or sets the content of the note.
        /// </summary>
        public string NoteContent { get; set; } = string.Empty;
    }
}