﻿using System;
using System.Collections.Generic;

namespace Infrastructure.EntityModels;

public partial class Trip
{
    public int TripId { get; set; }

    public string? Name { get; set; }

    public string Description { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal Budget { get; set; }

    public bool? IsActive { get; set; }

    public int NumberPeople { get; set; }

    public Guid? TripBackgroundGuid { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual Currency CurrencyCodeNavigation { get; set; } = null!;

    public virtual ICollection<LogBook> LogBooks { get; set; } = new List<LogBook>();

    public virtual ICollection<Medium> Media { get; set; } = new List<Medium>();
}
