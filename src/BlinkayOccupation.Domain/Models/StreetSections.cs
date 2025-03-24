using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class StreetSections
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

    public string Name { get; set; } = null!;

    public string Index { get; set; } = null!;

    public int From { get; set; }

    public int To { get; set; }

    public int Side { get; set; }

    public string ExternalId { get; set; } = null!;

    public string StreetId { get; set; } = null!;

    public DateTime Created { get; set; }

    public double? LocationLatitude { get; set; }

    public double? LocationLongitude { get; set; }

    public virtual ICollection<Capacities> Capacities { get; set; } = new List<Capacities>();

    public virtual Installations Installation { get; set; } = null!;

    public virtual ICollection<ParkingEvents> ParkingEvents { get; set; } = new List<ParkingEvents>();

    public virtual ICollection<ParkingRights> ParkingRights { get; set; } = new List<ParkingRights>();

    public virtual Shapes? Shape { get; set; }

    public virtual ICollection<Spaces> Spaces { get; set; } = new List<Spaces>();

    public virtual Streets Street { get; set; } = null!;

    public virtual ICollection<VehicleEvents> VehicleEvents { get; set; } = new List<VehicleEvents>();

    public virtual Zones Zone { get; set; } = null!;
}
