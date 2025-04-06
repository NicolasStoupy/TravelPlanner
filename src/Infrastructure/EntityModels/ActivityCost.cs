using System;
using System.Collections.Generic;

namespace Infrastructure.EntityModels;

public partial class ActivityCost
{
    public int ActivityCostId { get; set; }

    public string? Name { get; set; }

    public decimal Price { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public int TripId { get; set; }

    public int ActivityId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual Currency CurrencyCodeNavigation { get; set; } = null!;

    public virtual ICollection<Medium> Media { get; set; } = new List<Medium>();
}
