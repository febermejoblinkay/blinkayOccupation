using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class VehicleEvents
{
    public string Id { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime Date { get; set; }

    public string InstallationId { get; set; } = null!;

    public string ParkingEventId { get; set; } = null!;

    public int Direction { get; set; }

    public string ZoneId { get; set; } = null!;

    public string? StreetSectionId { get; set; }

    public string? SpaceId { get; set; }

    public string? DeviceId { get; set; }

    public DateTime DeviceDate { get; set; }

    public DateTime DeviceInserted { get; set; }

    public string Plate { get; set; } = null!;

    public string? Make { get; set; }

    public string? Model { get; set; }

    public string? Color { get; set; }

    public string? Nationality { get; set; }

    public string? Type { get; set; }

    public double? ConfidencePlate { get; set; }

    public double? ConfidenceMake { get; set; }

    public double? ConfidenceModel { get; set; }

    public double? ConfidenceColor { get; set; }

    public double? ConfidenceNationality { get; set; }

    public double? ConfidenceType { get; set; }

    public List<string>? Attachments { get; set; }

    public virtual Installations Installation { get; set; } = null!;

    public virtual ParkingEvents ParkingEvent { get; set; } = null!;

    public virtual Spaces? Space { get; set; }

    public virtual StreetSections? StreetSection { get; set; }

    public virtual Zones Zone { get; set; } = null!;
}
