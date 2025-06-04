using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Entities
{
    /// <summary>
    /// Represents a travel expense ticket, containing the file data and a unique identifier.
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// Gets or sets the unique identifier for the ticket.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the binary content of the ticket file (e.g., an image or PDF). Null if no file is attached.
        /// </summary>
        public byte[]? TicketFile { get; set; }

    }
}
