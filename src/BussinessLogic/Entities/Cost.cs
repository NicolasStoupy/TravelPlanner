using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Entities
{
    /// <summary>
    /// Represents an expense category with its details like name, price, currency, and associated ticket IDs.
    /// </summary>
    public class Cost
    {
        /// <summary>
        /// Gets or sets the unique identifier for the cost.
        /// </summary>
        public int CostID { get; set; }
        /// <summary>
        /// Gets or sets the descriptive name of the cost.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the amount associated with the cost.
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// Gets or sets the currency code or name for the cost.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the list of ticket file identifiers associated with this cost.
        /// </summary>
        public List<Guid> TicketsList { get; set; } = new List<Guid>();
    }
}
