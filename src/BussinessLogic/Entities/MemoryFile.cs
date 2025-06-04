using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Entities
{
    /// <summary>
    /// Represents a travel memory or souvenir, containing the file data, description, and associated metadata.
    /// </summary>
    public class MemoryFile
    {
        /// <summary>
        /// Gets or sets the unique identifier for the memory record.
        /// </summary>
        public int FileID { get; set; }

        /// <summary>
        /// Gets or sets the binary content of the memory (e.g., image or media data). Null if no content is available.
        /// </summary>
        public byte[]? Files { get; set; }

        /// <summary>
        /// Gets or sets an optional description or caption for the memory.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the memory is selected or checked in the UI.
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Gets or sets the unique GUID associated with the memory, used for tracking or retrieval.
        /// </summary>
        public Guid FileGuid { get; set; }
    }
}