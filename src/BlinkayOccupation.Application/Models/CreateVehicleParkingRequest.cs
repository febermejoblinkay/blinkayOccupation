using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace BlinkayOccupation.Application.Models
{
    public class CreateVehicleParkingRequest
    {
        public string InstallationId { get; set; }
        public DeviceInfo Device { get; set; }
        public ParkingAreaInfo ParkingArea { get; set; }
        public VehicleInfo Vehicle { get; set; }
        public int Direction { get; set; }

        [JsonIgnore]
        public IEnumerable<Blob>? Pictures { get; set; }
    }
}
