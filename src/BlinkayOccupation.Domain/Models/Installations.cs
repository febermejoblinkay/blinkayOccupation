using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class Installations
{
    public string Id { get; set; } = null!;

    public DateTime Updated { get; set; }

    public string ConfigurationTimeZoneId { get; set; } = null!;

    public TimeSpan ConfigurationMaxParkingEventDuration { get; set; }

    public string? ExternalId { get; set; }

    public string LanguageId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime Created { get; set; }

    public TimeSpan ConfigurationEnterGracePeriod { get; set; }

    public TimeSpan ConfigurationParkingRightMatchOffset { get; set; }

    public TimeSpan ConfigurationEventMatchingSpan { get; set; }

    public int ConfigurationParkingType { get; set; }

    public DateTime LastConsolidation { get; set; }

    public DateTime DateTimeNow()
    {
        return DateTime.SpecifyKind(ToInstallationDate(DateTime.UtcNow), DateTimeKind.Utc);
    }

    private DateTime ToInstallationDate(DateTime dateTime)
    {
        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, ConfigurationTimeZoneId);
    }

    public virtual ICollection<Attachments> Attachments { get; set; } = new List<Attachments>();

    public virtual ICollection<Capacities> Capacities { get; set; } = new List<Capacities>();

    public virtual ICollection<InputDevices> InputDevices { get; set; } = new List<InputDevices>();

    public virtual ICollection<Occupations> Occupations { get; set; } = new List<Occupations>();

    public virtual ICollection<ParkingEvents> ParkingEvents { get; set; } = new List<ParkingEvents>();

    public virtual ICollection<ParkingRights> ParkingRights { get; set; } = new List<ParkingRights>();

    public virtual ICollection<Spaces> Spaces { get; set; } = new List<Spaces>();

    public virtual ICollection<Stays> Stays { get; set; } = new List<Stays>();

    public virtual ICollection<StreetSections> StreetSections { get; set; } = new List<StreetSections>();

    public virtual ICollection<Streets> Streets { get; set; } = new List<Streets>();

    public virtual ICollection<Tariffs> Tariffs { get; set; } = new List<Tariffs>();

    public virtual ICollection<VehicleEvents> VehicleEvents { get; set; } = new List<VehicleEvents>();

    public virtual ICollection<Zones> Zones { get; set; } = new List<Zones>();
}
