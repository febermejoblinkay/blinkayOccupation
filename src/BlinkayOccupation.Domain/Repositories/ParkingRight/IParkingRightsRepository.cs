using BlinkayOccupation.Domain.Contexts;

namespace BlinkayOccupation.Domain.Repositories.ParkingRight
{
    public interface IParkingRightsRepository
    {
        Task<List<Models.ParkingRights>?> GetByIdsAsync(List<string> ids, BControlDbContext context);
    }
}
