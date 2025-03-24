using BlinkayOccupation.Domain.Contexts;

namespace BlinkayOccupation.Domain.Repositories.Zone
{
    public interface IZoneRepository
    {
        Task<Models.Zones> GetByIdAsync(string id, BControlDbContext context);
    }
}
