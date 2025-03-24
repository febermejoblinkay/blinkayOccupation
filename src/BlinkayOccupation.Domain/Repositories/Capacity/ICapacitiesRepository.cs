using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.Capacity
{
    public interface ICapacitiesRepository
    {
        Task<Capacities> GetAvailableCapacities(string installationId, string zoneId, string? tariffId, DateTime? entryDate, DateTime? exitDate, BControlDbContext context);
    }
}
