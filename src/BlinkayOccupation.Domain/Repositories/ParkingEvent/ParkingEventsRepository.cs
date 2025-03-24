using BlinkayOccupation.Domain.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlinkayOccupation.Domain.Repositories.ParkingEvent
{
    public class ParkingEventsRepository : IParkingEventsRepository
    {
        public async Task<Models.ParkingEvents?> GetByIdAsync(string id, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("The Id cannot be null or empty.", nameof(id));

            return await context.ParkingEvents.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Models.ParkingEvents?>> GetByIdsAsync(List<string> ids, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (ids?.Count == 0) throw new ArgumentException("Ids cannot be null or empty.", nameof(ids));

            return await context.ParkingEvents.Where(x => ids.Contains(x.Id)).ToListAsync();
        }
    }
}
