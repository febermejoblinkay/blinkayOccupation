using BlinkayOccupation.Application.Models;

namespace BlinkayOccupation.Application.Services.ParkingEvent
{
    public interface IParkingEventsService
    {
        Task<string> CreateParkingEvent(CreateVehicleParkingRequest request);
    }
}
