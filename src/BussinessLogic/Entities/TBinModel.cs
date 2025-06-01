using Infrastructure.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Entities
{
    /// <summary>
    /// Represents a transport bin model that holds trip details and associated media files.
    /// </summary>
    public class TBinModel
    {
        /// <summary>
        /// Gets or sets the trip information associated with this model.
        /// </summary>
        public Trip trip { get; set; } = new Trip();

        /// <summary>
        /// Gets or sets a collection of media files (e.g., photos, videos) related to the trip,
        /// where each entry is keyed by its unique identifier (GUID) and contains the raw byte data.
        /// </summary>
        public Dictionary<Guid, byte[]> medias { get; set; } = new Dictionary<Guid, byte[]>();
    }
}
