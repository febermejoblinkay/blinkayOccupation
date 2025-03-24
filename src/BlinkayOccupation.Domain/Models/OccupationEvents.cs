using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class OccupationEvents
{
    public string? Peid { get; set; }

    public string? EnterVehiclePlate { get; set; }

    public string? EnterVehicleType { get; set; }

    public string? EnterVehicleColor { get; set; }

    public string? EnterVehicleMake { get; set; }

    public string? EnterVehicleModel { get; set; }

    public string? EnterVehicleNationality { get; set; }

    public double? EnterVehicleConfidencePlate { get; set; }

    public double? EnterVehicleConfidenceType { get; set; }

    public double? EnterVehicleConfidenceColor { get; set; }

    public double? EnterVehicleConfidenceMake { get; set; }

    public double? EnterVehicleConfidenceModel { get; set; }

    public double? EnterVehicleConfidenceNationality { get; set; }

    public string? EnterAttachments { get; set; }

    public string? ExitVehiclePlate { get; set; }

    public string? ExitVehicleType { get; set; }

    public string? ExitVehicleColor { get; set; }

    public string? ExitVehicleMake { get; set; }

    public string? ExitVehicleModel { get; set; }

    public string? ExitVehicleNationality { get; set; }

    public double? ExitVehicleConfidencePlate { get; set; }

    public double? ExitVehicleConfidenceType { get; set; }

    public double? ExitVehicleConfidenceColor { get; set; }

    public double? ExitVehicleConfidenceMake { get; set; }

    public double? ExitVehicleConfidenceModel { get; set; }

    public double? ExitVehicleConfidenceNationality { get; set; }

    public string? ExitAttachments { get; set; }

    public DateTime? Updated { get; set; }

    public DateTime? Updatelocal { get; set; }

    public string? ZoneId { get; set; }

    public string? SpaceId { get; set; }

    public string? InstallationId { get; set; }

    public DateTime? Enter { get; set; }

    public DateTime? Exit { get; set; }

    public string? Prid { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public string? TariffId { get; set; }

    public DateTime? Prcreated { get; set; }

    public string? CurrencyId { get; set; }

    public decimal? Ammount { get; set; }

    public TimeSpan? Unpaidtolerance { get; set; }

    public TimeSpan? Expiredtolerance { get; set; }

    public DateTime? Utcnow { get; set; }

    public TimeSpan? Utcoffset { get; set; }

    public string? Plate { get; set; }
}
