using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.StayParkingRight
{
    public interface IStayParkingRightsRepository
    {
        Task AddRangeAsync(List<StaysParkingRights> stayParkingRight, BControlDbContext context);
        Task UpdateAsync(StaysParkingRights staysParkingRight, BControlDbContext context);
        Task<List<StaysParkingRights>> GetByStayIdAsync(string stayId, BControlDbContext context);
        Task<List<StaysParkingRights>> CheckParkingRightExistsInOtherStayAsync(List<string> pkRightIds, BControlDbContext context);
    }
}
