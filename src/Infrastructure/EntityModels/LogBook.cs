using System;
using System.Collections.Generic;

namespace Infrastructure.EntityModels;

public partial class LogBook
{
    public int LogBookId { get; set; }

    public string Description { get; set; } = null!;

    public int? TripLogBook { get; set; }

    public int? TripId { get; set; }

    public int? ActivityId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Activity? Activity { get; set; }

    public virtual Trip? TripLogBookNavigation { get; set; }
}
