using System;
using System.Collections.Generic;

namespace Infrastructure.EntityModels;

public partial class MediaType
{
    public int MediaType1 { get; set; }

    public string Description { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Medium> Media { get; set; } = new List<Medium>();
}
