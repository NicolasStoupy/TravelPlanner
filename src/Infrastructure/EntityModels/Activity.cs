using System;
using System.Collections.Generic;

namespace Infrastructure.EntityModels;

public partial class Activity
{
    public int TripId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Sequence { get; set; }

    public string? GoogleLink { get; set; }

    public decimal? PlannedCost { get; set; }

    public int ActivityTypeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int ActivityId { get; set; }

    public virtual ICollection<ActivityCost> ActivityCosts { get; set; } = new List<ActivityCost>();

    public virtual ActivityType ActivityType { get; set; } = null!;

    public virtual ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();

    public virtual ICollection<LogBook> LogBooks { get; set; } = new List<LogBook>();

    public virtual Trip Trip { get; set; } = null!;
}
