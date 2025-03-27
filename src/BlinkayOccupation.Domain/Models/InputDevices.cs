namespace BlinkayOccupation.Domain.Models;

public partial class InputDevices
{
    public string Id { get; set; } = null!;

    public string InstallationId { get; set; } = null!;

    public int Capabilities { get; set; }

    public int HardwareType { get; set; }

    public bool Disabled { get; set; }

    public string? DeviceInformation { get; set; }

    public int? StatusStatus { get; set; }

    public DateTime? StatusLastUpdated { get; set; }

    public string? StatusMetadata { get; set; }

    public DateTime? ConfigurationLastUpdated { get; set; }

    public string? ConfigurationValue { get; set; }

    public double? LocationLatitude { get; set; }

    public double? LocationLongitude { get; set; }

    public int CameraType { get; set; }

    public DateTime LastReceivedEvent { get; set; }

    public string? ParkingAreas { get; set; }

    public virtual ICollection<Attachments> Attachments { get; set; } = new List<Attachments>();

    public virtual Installations Installation { get; set; } = null!;
}
