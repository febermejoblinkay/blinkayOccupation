using BlinkayOccupation.Domain.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlinkayOccupation.Domain.Repositories.ParkingRight
{
    public class ParkingRightsRepository : IParkingRightsRepository
    {
        public async Task<List<Models.ParkingRights>?> GetByIdsAsync(List<string> ids, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (ids.Count == 0) throw new ArgumentException("Ids cannot be null or empty.", nameof(ids));

            return await context.ParkingRights.Where(x => ids.Contains(x.Id)).ToListAsync();
        }
    }
}
