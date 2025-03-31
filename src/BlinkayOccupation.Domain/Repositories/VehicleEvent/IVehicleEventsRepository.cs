using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.VehicleEvent
{
    public interface IVehicleEventsRepository
    {
        Task<Models.VehicleEvents?> GetByPlateAndOrDirectionAndOrDate(string plate, int direction, DateTime startDate, DateTime endDate, BControlDbContext context);
        Task<List<Models.VehicleEvents>?> GetDuplicateByPlateAndOrDirectionAndOrDate(string plate, int direction, DateTime date, BControlDbContext context);
        Task RemoveAsync(VehicleEvents vehicleEvent, BControlDbContext context);
    }
}
