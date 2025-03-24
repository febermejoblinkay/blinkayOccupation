using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class Capacities
{
    public string Id { get; set; } = null!;

    public DateTime Updated { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public string InstallationId { get; set; } = null!;

    public string ZoneId { get; set; } = null!;

    public string? StreetSectionId { get; set; }

    public string? SpaceId { get; set; }

    public int Count { get; set; }

    public string TariffId { get; set; } = null!;

    public DateTime Created { get; set; }

    public virtual Installations Installation { get; set; } = null!;

    public virtual Spaces? Space { get; set; }

    public virtual StreetSections? StreetSection { get; set; }

    public virtual Tariffs Tariff { get; set; } = null!;

    public virtual Zones Zone { get; set; } = null!;
}
