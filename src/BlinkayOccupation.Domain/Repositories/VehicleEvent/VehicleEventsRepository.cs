using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlinkayOccupation.Domain.Repositories.VehicleEvent
{
    public class VehicleEventsRepository : IVehicleEventsRepository
    {
        private const int FUZZY_DISTANCE = 1;
        private static readonly TimeSpan _duplicateSpan = TimeSpan.FromSeconds(15);

        public async Task<Models.VehicleEvents?> GetByPlateAndOrDirectionAndOrDate(string plate, int direction, DateTime startDate, DateTime endDate, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var query = context.VehicleEvents.Where(x => x.Direction == direction && x.Date >= startDate && x.Date < endDate && EF.Functions.FuzzyStringMatchLevenshteinLessEqual(x.Plate, plate, FUZZY_DISTANCE) <= FUZZY_DISTANCE);
            return direction == 1
                ? await query.OrderBy(x => x.Date).FirstOrDefaultAsync()
                : await query.OrderByDescending(x => x.Date).FirstOrDefaultAsync();
        }
        
        public async Task<List<Models.VehicleEvents>?> GetDuplicateByPlateAndOrDirectionAndOrDate(string plate, int direction, DateTime date, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var offset = date - _duplicateSpan;
            return await context.VehicleEvents.Where(x => x.Direction == direction && x.Date >= offset && EF.Functions.FuzzyStringMatchLevenshteinLessEqual(x.Plate, plate, FUZZY_DISTANCE) <= FUZZY_DISTANCE).ToListAsync();
        }

        public async Task RemoveAsync(VehicleEvents vehicleEvent, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (vehicleEvent == null) throw new ArgumentException("vehicleEvent Object can not be null.", nameof(vehicleEvent));

            context.VehicleEvents.Remove(vehicleEvent);
            await context.SaveChangesAsync();
        }
    }
}
