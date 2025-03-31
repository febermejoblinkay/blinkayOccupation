using BlinkayOccupation.Domain.Contexts;

namespace BlinkayOccupation.Domain.Repositories.ParkingRight
{
    public interface IParkingRightsRepository
    {
        Task<List<Models.ParkingRights>?> GetByExternalIdsAsync(List<string> ids, BControlDbContext context);
        Task<Models.ParkingRights?> GetByValidPlateAsync(string plate, DateTime date, TimeSpan expandBy, BControlDbContext context);
    }
}
