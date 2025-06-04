using CommunityToolkit.Mvvm.ComponentModel;

namespace BussinessLogic.Entities
{
    /// <summary>
    /// Represents a travel record with associated activities, costs, followers, tickets, memories, and notes.
    /// </summary>
    public class Travel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the travel.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the travel.
        /// </summary>
        public string? name { get; set; }

        /// <summary>
        /// Gets or sets the GUID of the cover image associated with the travel.
        /// </summary>
        public Guid? imageID { get; set; }

        /// <summary>
        /// Gets or sets the binary content of the cover image. Null if no image is available.
        /// </summary>
        public byte[]? image { get; set; }

        /// <summary>
        /// Gets or sets the total budget for the travel.
        /// </summary>
        public decimal budget { get; set; }

        /// <summary>
        /// Gets or sets the number of people participating in the travel.
        /// </summary>
        public int people { get; set; }

        /// <summary>
        /// Gets or sets an optional description of the travel.
        /// </summary>
        public string? description { get; set; }

        /// <summary>
        /// Gets or sets the currency code used for the travel costs.
        /// </summary>
        public string? currencie { get; set; }

        /// <summary>
        /// Gets or sets the date when the travel was created or scheduled. Defaults to the current date and time.
        /// </summary>
        public DateTime travelDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the start date of the travel. Defaults to the current date and time.
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the end date of the travel. Defaults to the current date and time.
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the collection of activities associated with this travel.
        /// </summary>
        public List<TravelActivity> TravelActivities { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of notes added for this travel.
        /// </summary>
        public List<Note> TravelNotes { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of cost entries associated with this travel.
        /// </summary>
        public List<Cost> TravelCosts { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of followers (participants) for this travel.
        /// </summary>
        public List<Follower> Followers { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of tickets related to expenses for this travel.
        /// </summary>
        public List<Ticket> TravelTickets { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of memory files (souvenirs) for this travel.
        /// </summary>
        public List<MemoryFile> MemoryFiles { get; set; } = new();

        /// <summary>
        /// Gets the total number of notes associated with this travel.
        /// </summary>
        public int CountNote => TravelNotes.Count();

        /// <summary>
        /// Gets the total cost by summing all cost entries for this travel.
        /// </summary>
        public double TotalCost => TravelCosts.Sum(c => c.Price);

        /// <summary>
        /// Gets the total number of followers (participants) for this travel.
        /// </summary>
        public int CountFollowers => Followers.Count();

        /// <summary>
        /// Gets the total number of tickets associated with this travel.
        /// </summary>
        public int CountTickets => TravelTickets.Count();

        /// <summary>
        /// Gets the total number of activities included in this travel.
        /// </summary>
        public int CountActivities => TravelActivities.Count();
    }
}