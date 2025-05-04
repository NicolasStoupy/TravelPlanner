

namespace BussinessLogic.Entities
{
    public class TravelActivity
    {
        public int TravelID { get; set; }
        public int ActivityID { get; set; }
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int Sequence { get; set; }

        public string? GoogleLink { get; set; }

        public decimal? PlannedCost { get; set; }

        public TypeOfActivity ActivityType { get; set; }

        public string ActivityTypeName { get; set; }


        public List<Follower> Followers { get; set; }= new();

        public List<Note> Notes { get; set;} = new();

        public List<Cost> Cost { get; set; } = new();

        public List<Ticket> Tickets { get; set; } = new();
        public DateTime ActivityDate { get;  set; }
    }
}
