using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class Streets
{
    public string Id { get; set; } = null!;

    public DateTime Updated { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public string InstallationId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Index { get; set; } = null!;

    public string? ExternalId { get; set; }

    public DateTime Created { get; set; }

    public virtual Installations Installation { get; set; } = null!;

    public virtual ICollection<StreetSections> StreetSections { get; set; } = new List<StreetSections>();
}
