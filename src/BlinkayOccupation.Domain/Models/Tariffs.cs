using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class Tariffs
{
    public string Id { get; set; } = null!;

    public DateTime Updated { get; set; }

    public string InstallationId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime Created { get; set; }

    public TimeSpan ExitGracePeriod { get; set; }

    public bool IsDefault { get; set; }
    
    public bool? PaymentApplyAllDay { get; set; }

    public List<string>? VehicleMakes { get; set; }

    public bool IsResident { get; set; }

    public virtual ICollection<Capacities> Capacities { get; set; } = new List<Capacities>();

    public virtual Installations Installation { get; set; } = null!;

    public virtual ICollection<Occupations> Occupations { get; set; } = new List<Occupations>();

    public virtual ICollection<OccupationsSnapshots> OccupationsSnapshots { get; set; } = new List<OccupationsSnapshots>();

    public virtual ICollection<ParkingEvents> ParkingEvents { get; set; } = new List<ParkingEvents>();

    public virtual ICollection<ParkingRights> ParkingRights { get; set; } = new List<ParkingRights>();
}
