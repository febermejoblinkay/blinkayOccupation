namespace BlinkayOccupation.Domain.Models;

public partial class Blobs
{
    public string Id { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public DateTime Created { get; set; }

    public byte[] Data { get; set; } = null!;

    public string? Metadata { get; set; }
}
