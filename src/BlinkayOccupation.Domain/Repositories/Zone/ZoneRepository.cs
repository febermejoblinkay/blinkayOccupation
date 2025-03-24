using BlinkayOccupation.Domain.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlinkayOccupation.Domain.Repositories.Zone
{
    public class ZoneRepository : IZoneRepository
    {
        public async Task<Models.Zones> GetByIdAsync(string id, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id can not be null or empty.", nameof(id));

            return await context.Zones.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
