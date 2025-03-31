using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Application.Models
{
    public class ParkingAreaInfo
    {
        public string ZoneId { get; set; }
        public string? StreetSectionId { get; set; }
        public string? SpaceId { get; set; }
    }
}
