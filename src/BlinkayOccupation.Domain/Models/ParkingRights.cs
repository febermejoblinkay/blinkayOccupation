using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class ParkingRights
{
    public string Id { get; set; } = null!;

    public DateTime Updated { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public string InstallationId { get; set; } = null!;

    public List<string>? Plates { get; set; }

    public string ZoneId { get; set; } = null!;

    public string? StreetSectionId { get; set; }

    public string? SpaceId { get; set; }

    public DateTime Created { get; set; }

    public TimeSpan TotalTime { get; set; }

    public double Amount { get; set; }

    public string CurrencyId { get; set; } = null!;

    public string? PaymentSource { get; set; }

    public string TariffId { get; set; } = null!;

    public int State { get; set; }

    public bool Deleted { get; set; }

    public string? Email { get; set; }

    public string ExternalId { get; set; } = null!;

    public virtual Installations Installation { get; set; } = null!;

    public virtual ICollection<ParkingEvents> ParkingEvents { get; set; } = new List<ParkingEvents>();

    public virtual Spaces? Space { get; set; }

    public virtual ICollection<StaysParkingRights> StaysParkingRights { get; set; } = new List<StaysParkingRights>();

    public virtual StreetSections? StreetSection { get; set; }

    public virtual Tariffs Tariff { get; set; } = null!;

    public virtual Zones Zone { get; set; } = null!;
}
