namespace Infrastructure.EntityModels;

public partial class Attendee
{
    public int AttendeeId { get; set; }

    public string Name { get; set; } = null!;

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public int TripId { get; set; }

    public int ActivityId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Activity Activity { get; set; } = null!;
}
