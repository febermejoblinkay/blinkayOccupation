namespace BlinkayOccupation.Application.Models
{
    public class VehicleInfo
    {
        public string? Plate { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? Color { get; set; }
        public string? Nationality { get; set; }
        public string? Type { get; set; }
        public ConfidenceInfo Confidence { get; set; }
    }
}
