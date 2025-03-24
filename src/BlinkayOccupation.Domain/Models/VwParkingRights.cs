using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class VwParkingRights
{
    public string? Id { get; set; }

    public DateTime? Updated { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public string? InstallationId { get; set; }

    public string? Plates { get; set; }

    public string? ZoneId { get; set; }

    public string? StreetSectionId { get; set; }

    public string? SpaceId { get; set; }

    public DateTime? Created { get; set; }

    public TimeSpan? TotalTime { get; set; }

    public double? Amount { get; set; }

    public string? CurrencyId { get; set; }

    public string? PaymentSource { get; set; }

    public string? TariffId { get; set; }

    public int? State { get; set; }

    public bool? Deleted { get; set; }

    public string? Email { get; set; }

    public string? ExternalId { get; set; }
}
