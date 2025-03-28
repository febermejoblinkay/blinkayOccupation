using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlinkayOccupation.Domain.Repositories.Occupation
{
    public class OccupationRepository : IOccupationRepository
    {
        public async Task AddAsync(Occupations occupation, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (occupation == null) throw new ArgumentException("Occupation Object can not be null.", nameof(occupation));

            await context.Occupations.AddAsync(occupation);
            //await context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<Occupations> occupations, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (occupations?.Count == 0) throw new ArgumentException("Occupations list can not be null.", nameof(occupations));

            context.Occupations.AddRangeAsync(occupations);
            //await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Occupations occupation, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (occupation == null) throw new ArgumentException("Occupation Object can not be null.", nameof(occupation));

            context.Occupations.Update(occupation);
            //await context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(List<Occupations> occupations, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (occupations?.Count == 0) throw new ArgumentException("Occupations list can not be null.", nameof(occupations));

            context.Occupations.UpdateRange(occupations);
            //await context.SaveChangesAsync();
        }

        public async Task<List<Occupations>?> GetOccupationsAvailable(
            DateTime date,
            string installationId,
            string zoneId,
            string? tariffId,
            BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (date == default(DateTime) ||
                string.IsNullOrWhiteSpace(installationId) ||
                string.IsNullOrWhiteSpace(zoneId))
            {
                throw new ArgumentException("Parameters are null or empty.");
            }

            return await context.Occupations.Where(o =>
                o.Date.Value.Date == date.Date &&
                o.InstallationId == installationId &&
                o.ZoneId == zoneId &&
                !o.Deleted &&
                (o.TariffId == tariffId || o.TariffId == null)
            ).ToListAsync();
        }

        public async Task<List<Occupations>?> GetExistingOccupationsByDate(
            DateTime fromDate,
            DateTime toDate,
            string installationId,
            string zoneId,
            string? tariffId,
            BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (fromDate == default ||
                toDate == default ||
                string.IsNullOrWhiteSpace(installationId) ||
                string.IsNullOrWhiteSpace(zoneId))
            {
                throw new ArgumentException("Parameters are null or empty.");
            }

            return await context.Occupations
                .Where(o =>
                    o.Date.Value.Date >= fromDate.Date &&  
                    o.Date.Value.Date <= toDate.Date &&
                    o.InstallationId == installationId &&
                    o.ZoneId == zoneId &&
                    !o.Deleted &&
                    (o.TariffId == tariffId || o.TariffId == null))
                .ToListAsync();
        }
    }
}
