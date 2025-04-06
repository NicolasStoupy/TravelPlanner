using System;
using System.Collections.Generic;

namespace Infrastructure.EntityModels;

public partial class Currency
{
    public string CurrencyCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Symbol { get; set; } = null!;

    public decimal? ExchangeRate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ActivityCost> ActivityCosts { get; set; } = new List<ActivityCost>();

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
