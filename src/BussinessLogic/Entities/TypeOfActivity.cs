using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Entities
{
    /// <summary>
    /// Represents a category or classification of an activity within a travel itinerary.
    /// </summary>
    public class TypeOfActivity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the activity type.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the descriptive name of the activity type.
        /// </summary>
        public string Name { get; set; }
    }
}
