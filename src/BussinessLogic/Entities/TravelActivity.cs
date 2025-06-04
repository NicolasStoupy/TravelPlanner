

namespace BussinessLogic.Entities
{
    /// <summary>
    /// Represents an activity within a travel, including details like name, sequence, costs, and associated entities.
    /// </summary>
    public class TravelActivity
    {
        /// <summary>
        /// Gets or sets the identifier of the travel this activity belongs to.
        /// </summary>
        public int TravelID { get; set; }
        /// <summary>
        /// Gets or sets the unique identifier for the activity.
        /// </summary>
        public int ActivityID { get; set; }
        /// <summary>
        /// Gets or sets the name of the activity.
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Gets or sets an optional description of the activity.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Gets or sets the sequence order of the activity within the travel.
        /// </summary>
        public int Sequence { get; set; }
        /// <summary>
        /// Gets or sets a Google Maps link associated with this activity, if available.
        /// </summary>
        public string? GoogleLink { get; set; }
        /// <summary>
        /// Gets or sets the planned cost for the activity, if specified.
        /// </summary>
        public decimal? PlannedCost { get; set; }
        /// <summary>
        /// Gets or sets the type of activity (e.g., sightseeing, dining).
        /// </summary
        public TypeOfActivity ActivityType { get; set; }
        /// <summary>
        /// Gets or sets the display name of the activity type.
        /// </summary>
        public string ActivityTypeName { get; set; }

        /// <summary>
        /// Gets or sets the collection of followers (participants) for this activity.
        /// </summary>
        public List<Follower> Followers { get; set; }= new();
        /// <summary>
        /// Gets or sets the collection of notes associated with this activity.
        /// </summary>
        public List<Note> Notes { get; set;} = new();
        /// <summary>
        /// Gets or sets the collection of cost entries related to this activity.
        /// </summary>
        public List<Cost> Cost { get; set; } = new();
        /// <summary>
        /// Gets or sets the collection of tickets associated with expenses for this activity.
        /// </summary>
        public List<Ticket> Tickets { get; set; } = new();
        /// <summary>
        /// Gets or sets the date on which this activity occurs.
        /// </summary>
        public DateTime ActivityDate { get;  set; }
        /// <summary>
        /// Gets or sets the total amount spent for this activity (calculated or stored).
        /// </summary>
        public double Total { get; internal set; }
    }
}
