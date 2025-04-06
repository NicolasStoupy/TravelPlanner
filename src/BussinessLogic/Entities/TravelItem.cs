
namespace BussinessLogic.Entities
{
    public class TravelItem
    {
        public int Id { get; set; }

        public string? name { get; set; }

        public byte[]? image { get; set; }

        public string? description { get; set; }

        public DateOnly travelDate { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }
    }
}
