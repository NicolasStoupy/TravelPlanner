﻿using System;
using System.Collections.Generic;

namespace Infrastructure.EntityModels;

public partial class Medium
{
    public int MediaId { get; set; }

    public Guid FileGuid { get; set; }

    public string Description { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public int? ActivityCostId { get; set; }

    public int MediaType { get; set; }

    public int? TripId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ActivityCost? ActivityCost { get; set; }

    public virtual MediaType MediaTypeNavigation { get; set; } = null!;

    public virtual Trip? Trip { get; set; }
}
