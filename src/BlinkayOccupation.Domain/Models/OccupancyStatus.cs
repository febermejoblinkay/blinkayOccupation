using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class OccupancyStatus
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public double? LocationLatitude { get; set; }

    public double? LocationLongitude { get; set; }

    public string? Peid { get; set; }

    public DateTime? Enter { get; set; }

    public string? Prid { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public TimeSpan? ExitGracePeriod { get; set; }

    public string? Zoneid { get; set; }

    public string? Ssectionid { get; set; }

    public TimeSpan? ConfigurationEnterGracePeriod { get; set; }

    public string? ConfigurationTimeZoneId { get; set; }

    public string? Installationid { get; set; }

    public string? Attachmentsstorageid { get; set; }

    public TimeSpan? Utcoffset { get; set; }

    public DateTime? Utcnow { get; set; }
}
