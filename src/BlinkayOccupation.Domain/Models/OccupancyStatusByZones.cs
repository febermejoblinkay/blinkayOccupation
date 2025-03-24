using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class OccupancyStatusByZones
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public double? LocationLatitude { get; set; }

    public double? LocationLongitude { get; set; }

    public byte[]? Geometry { get; set; }

    public string? InstallationId { get; set; }

    public long? Occupancy { get; set; }

    public string? Device { get; set; }

    public string? Attachmentsstorageid { get; set; }

    public DateTime? Attachmentsstoragedatetime { get; set; }

    public long? Capacity { get; set; }
}
