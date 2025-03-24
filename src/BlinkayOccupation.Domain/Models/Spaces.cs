using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class Spaces
{
    public string Id { get; set; } = null!;

    public DateTime Updated { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public double? BoundingBoxLeft { get; set; }

    public double? BoundingBoxTop { get; set; }

    public double? BoundingBoxRight { get; set; }

    public double? BoundingBoxBottom { get; set; }

    public string? ShapeId { get; set; }

    public int Type { get; set; }

    public string InstallationId { get; set; } = null!;

    public string ZoneId { get; set; } = null!;

    public string? StreetSectionId { get; set; }

    public string Name { get; set; } = null!;

    public string Index { get; set; } = null!;

    public string ExternalId { get; set; } = null!;

    public DateTime Created { get; set; }

    public double? LocationLatitude { get; set; }

    public double? LocationLongitude { get; set; }

    public virtual ICollection<Capacities> Capacities { get; set; } = new List<Capacities>();

    public virtual Installations Installation { get; set; } = null!;

    public virtual ICollection<ParkingEvents> ParkingEvents { get; set; } = new List<ParkingEvents>();

    public virtual ICollection<ParkingRights> ParkingRights { get; set; } = new List<ParkingRights>();

    public virtual Shapes? Shape { get; set; }

    public virtual StreetSections? StreetSection { get; set; }

    public virtual ICollection<VehicleEvents> VehicleEvents { get; set; } = new List<VehicleEvents>();

    public virtual Zones Zone { get; set; } = null!;
}
