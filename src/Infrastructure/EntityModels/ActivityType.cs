using System;
using System.Collections.Generic;

namespace Infrastructure.EntityModels;

public partial class ActivityType
{
    public int ActivityTypeId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
