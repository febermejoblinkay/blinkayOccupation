using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class ConsolidatedOccupation
{
    public long Id { get; set; }

    public DateTime Date { get; set; }

    public string InstallationId { get; set; } = null!;

    public string InstallationValue { get; set; } = null!;

    public string? ZoneId { get; set; }

    public string? ZoneValue { get; set; }

    public string? SectionId { get; set; }

    public string? SectionValue { get; set; }

    public string TariffId { get; set; } = null!;

    public string TariffValue { get; set; } = null!;

    public int Paid { get; set; }

    public int Occupied { get; set; }

    public int Sold { get; set; }

    public int Total { get; set; }
}
