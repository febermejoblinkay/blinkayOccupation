using BlinkayOccupation.Domain.Contexts;

namespace BlinkayOccupation.Domain.Repositories.ParkingEvent
{
    public interface IParkingEventsRepository
    {
        Task<Models.ParkingEvents?> GetByIdAsync(string id, BControlDbContext context);
        Task<List<Models.ParkingEvents?>> GetByIdsAsync(List<string> ids, BControlDbContext context);
    }
}
