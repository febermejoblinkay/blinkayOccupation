using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class Shapes
{
    public string Id { get; set; } = null!;

    public DateTime Updated { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public byte[] Geometry { get; set; } = null!;

    public string? ExternalId { get; set; }

    public DateTime Created { get; set; }

    public virtual Spaces? Spaces { get; set; }

    public virtual StreetSections? StreetSections { get; set; }

    public virtual Zones? Zones { get; set; }
}
