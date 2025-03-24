using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlinkayOccupation.Domain.Repositories.Capacity
{
    public class CapacitiesRepository : ICapacitiesRepository
    {
        public async Task<Capacities> GetAvailableCapacities(string installationId, string zoneId, string? tariffId, DateTime? entryDate, DateTime? exitDate, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(installationId) || string.IsNullOrWhiteSpace(zoneId)) throw new ArgumentException("Parameters are null or empty.");

            return await context.Capacities.FirstOrDefaultAsync(x => 
                x.InstallationId == installationId && 
                x.ZoneId == zoneId &&
                ((entryDate.HasValue && entryDate.Value >= x.ValidFrom) || !entryDate.HasValue) ||
                ((exitDate.HasValue && exitDate.Value <= x.ValidTo) || !exitDate.HasValue) &&
                (x.TariffId == tariffId || x.Tariff == null));
        }
    }
}
