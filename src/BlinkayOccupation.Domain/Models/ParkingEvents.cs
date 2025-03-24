using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class ParkingEvents
{
    public string Id { get; set; } = null!;

    public DateTime Updated { get; set; }

    public string InstallationId { get; set; } = null!;

    public string? ParkingRightId { get; set; }

    public string? Plate { get; set; }

    public string? SpaceId { get; set; }

    public int Type { get; set; }

    public string? StreetSectionId { get; set; }

    public double? PlateConfidence { get; set; }

    public string ZoneId { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime? Enter { get; set; }

    public DateTime? Exit { get; set; }

    public string? VehicleId { get; set; }

    public string TariffId { get; set; } = null!;

    public string DeviceId { get; set; } = null!;

    public int ClosingReason { get; set; }

    public virtual Installations Installation { get; set; } = null!;

    public virtual ParkingRights? ParkingRight { get; set; }

    public virtual Spaces? Space { get; set; }

    public virtual ICollection<Stays> StaysEntryEvent { get; set; } = new List<Stays>();

    public virtual ICollection<Stays> StaysExitEvent { get; set; } = new List<Stays>();

    public virtual StreetSections? StreetSection { get; set; }

    public virtual Tariffs Tariff { get; set; } = null!;

    public virtual ICollection<VehicleEvents> VehicleEvents { get; set; } = new List<VehicleEvents>();

    public virtual Zones Zone { get; set; } = null!;
}
