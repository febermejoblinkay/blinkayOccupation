using BlinkayOccupation.Domain.Helpers;
using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class Stays : IAuditable
{
    public string Id { get; set; } = null!;

    public string? EntryEventId { get; set; }

    public string? ExitEventId { get; set; }

    public DateTime? EntryDate { get; set; }

    public DateTime? ExitDate { get; set; }

    public string? InstallationId { get; set; }

    public string ZoneId { get; set; } = null!;

    public int? CaseId { get; set; }

    public DateTime Created { get; set; }

    public bool Deleted { get; set; }

    public DateTime Updated { get; set; }

    public virtual ParkingEvents? EntryEvent { get; set; }

    public virtual ParkingEvents? ExitEvent { get; set; }

    public virtual Installations? Installation { get; set; }

    public virtual ICollection<StaysParkingRights> StaysParkingRights { get; set; } = new List<StaysParkingRights>();

    public virtual Zones Zone { get; set; } = null!;
}
