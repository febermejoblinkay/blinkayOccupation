using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlinkayOccupation.Domain.Repositories.StayParkingRight
{
    public class StayParkingRightsRepository : IStayParkingRightsRepository
    {
        public async Task AddRangeAsync(List<StaysParkingRights> stayParkingRight, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (stayParkingRight.Count == 0) throw new ArgumentException("StayParkingRight can not be null.", nameof(stayParkingRight));

            await context.StaysParkingRights.AddRangeAsync(stayParkingRight);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StaysParkingRights staysParkingRight, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (staysParkingRight == null) throw new ArgumentException("staysParkingRight Object can not be null.", nameof(staysParkingRight));

            context.StaysParkingRights.Update(staysParkingRight);
            await context.SaveChangesAsync();
        }

        public async Task<List<StaysParkingRights>> GetByStayIdAsync(string stayId, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(stayId)) throw new ArgumentException("stayId can not be null.", nameof(stayId));

            return await context.StaysParkingRights.Include(x => x.ParkingRight).Where(x => x.StayId == stayId).ToListAsync();
        }

        public async Task<List<StaysParkingRights>> CheckParkingRightExistsInOtherStayAsync(List<string> pkRightIds, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (pkRightIds?.Count == 0) throw new ArgumentException("stayIds can not be null.", nameof(pkRightIds));

            return await context.StaysParkingRights.Where(x => pkRightIds.Any(y => y == x.ParkingRightId)).ToListAsync();
        }
    }
}
