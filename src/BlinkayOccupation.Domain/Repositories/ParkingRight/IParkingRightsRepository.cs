using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.ParkingRight
{
    public interface IParkingRightsRepository
    {
        Task AddAsync(ParkingRights pRight, BControlDbContext context);
        Task<List<Models.ParkingRights>?> GetByExternalIdsAsync(List<string> ids, BControlDbContext context);
        Task<Models.ParkingRights?> GetByValidPlateAsync(string plate, DateTime date, TimeSpan expandBy, BControlDbContext context);
    }
}
