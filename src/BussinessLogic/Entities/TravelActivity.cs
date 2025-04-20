

namespace BussinessLogic.Entities
{
    public class TravelActivity
    {
     
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int Sequence { get; set; }

        public string? GoogleLink { get; set; }

        public decimal? PlannedCost { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.NotDefined;


        public List<Follower>? Followers { get; set; }

        public List<Note>? Notes { get; set;}

        public Cost? Cost { get; set; } 

        public List<Ticket>? Tickets { get; set; }

    }
}
