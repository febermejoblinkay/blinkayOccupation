using BlinkayOccupation.Domain.Helpers;
using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class Occupations : IAuditable
{
    public string Id { get; set; } = null!;

    public DateTime? Date { get; set; }

    public string? InstallationId { get; set; }

    public string? ZoneId { get; set; }

    public string? TariffId { get; set; }

    public int? PaidRealOccupation { get; set; }

    public int? UnpaidRealOccupation { get; set; }

    public int? PaidOccupation { get; set; }

    public int? Total { get; set; }

    public DateTime Created { get; set; }

    public bool Deleted { get; set; }

    public DateTime Updated { get; set; }

    public virtual Installations? Installation { get; set; }

    public virtual Tariffs? Tariff { get; set; }

    public virtual Zones? Zone { get; set; }
}
