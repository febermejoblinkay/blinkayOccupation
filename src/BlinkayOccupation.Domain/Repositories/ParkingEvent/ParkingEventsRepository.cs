using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using Microsoft.EntityFrameworkCore;
using static BlinkayOccupation.Domain.Repositories.ParkingEvent.IParkingEventsRepository;

namespace BlinkayOccupation.Domain.Repositories.ParkingEvent
{
    public class ParkingEventsRepository : IParkingEventsRepository
    {
        private const int FUZZY_DISTANCE = 1;

        public async Task<ParkingEvents?> GetByIdAsync(string id, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("The Id cannot be null or empty.", nameof(id));

            return await context.ParkingEvents
                .Include(x => x.Installation)
                .Include(x => x.ParkingRight)
                    //.ThenInclude(x => x.)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ParkingEvents?>> GetByIdsAsync(List<string> ids, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (ids?.Count == 0) throw new ArgumentException("Ids cannot be null or empty.", nameof(ids));

            return await context.ParkingEvents.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task UpdateAsync(ParkingEvents pEvent, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (pEvent == null) throw new ArgumentException("pEvent Object can not be null.", nameof(pEvent));

            context.ParkingEvents.Update(pEvent);
            await context.SaveChangesAsync();
        }

        public async Task AddAsync(ParkingEvents pEvent, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (pEvent == null) throw new ArgumentException("pEvent Object can not be null.", nameof(pEvent));

            context.ParkingEvents.Add(pEvent);
            await context.SaveChangesAsync();
        }

        public async Task<List<ParkingEvents>> ListOpenByPlate(OpenFrom openFrom, IEnumerable<string> plates, BControlDbContext context)
        {
            var queryable = FindOpenFromQueryable(openFrom, context);
            var open = await queryable.Where(x => x.Plate != null && plates.Contains(x.Plate)).OrderBy(x => x.Enter).ToListAsync();

            if (open.Count == 0)
            {
                open = new(2);
                foreach (var plate in plates)
                {
                    var queryables = FindOpenFromQueryable(openFrom, context);
                    open.AddRange(await queryable.Where(x => x.Plate != null && EF.Functions.FuzzyStringMatchLevenshteinLessEqual(x.Plate, plate, FUZZY_DISTANCE) <= FUZZY_DISTANCE).OrderBy(x => x.Enter).ToListAsync());
                }
            }

            return open;
        }

        private IQueryable<ParkingEvents> FindOpenFromQueryable(OpenFrom openFrom, BControlDbContext context)
        {
            return context.ParkingEvents.Where(x => x.Exit == null && x.Enter != null && x.Enter >= openFrom.From && openFrom.Now - x.Enter <= openFrom.MaxParkingEventDuration);
        }
    }
}
