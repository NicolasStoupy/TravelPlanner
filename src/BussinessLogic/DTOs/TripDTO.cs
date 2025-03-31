
namespace BussinessLogic.DTOs
{
    public class TripDTO
    {
        public string? Name { get; set; }

        public string? Description { get; set; }
        public float Budget { get; set; }

        public int NumberPeople { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string? CurrencyCode { get; set; }
    }
}
