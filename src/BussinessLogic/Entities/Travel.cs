
using CommunityToolkit.Mvvm.ComponentModel;

namespace BussinessLogic.Entities
{

    public  class Travel
    {
        public int Id { get; set; }

        public string? name { get; set; }

        public Guid? imageID { get; set; }
        public byte[]? image { get; set; }

        public decimal budget { get; set; }

        public int people { get; set; }

        public string? description { get; set; }

        public string? currencie { get; set; }
        public DateTime travelDate { get; set; } = DateTime.Now;

        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime EndDate { get; set; } = DateTime.Now;

        public List<TravelActivity> TravelActivities { get; set; } = new();

        public List<Note> TravelNotes { get; set; } = new();

        public List<Cost> TravelCosts { get; set; } = new();

        public List<Follower>? Followers { get; set; }

        public List<Ticket>? TravelTickets { get; set; } = new();
    }
}
