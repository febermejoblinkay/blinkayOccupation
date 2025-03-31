namespace BlinkayOccupation.Application.Models
{
    public sealed record Blob(string ContentType, Stream Stream, IReadOnlyDictionary<string, string>? Metadata = null);
}
