using BlinkayOccupation.Domain.Contexts;

namespace BlinkayOccupation.Domain.Repositories.ParkingRight
{
    public interface IParkingRightsRepository
    {
        Task<List<Models.ParkingRights>?> GetByExternalIdsAsync(List<string> ids, BControlDbContext context);
    }
}
