using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlinkayOccupation.Domain.Repositories.Tariff
{
    public class TariffRepository : ITariffRepository
    {
        public async Task<List<Tariffs>> GetTariffByInsId(string insId, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            return await context.Tariffs.Where(x => x.InstallationId == insId).ToListAsync();
        }

        public async Task<Tariffs?> FindByVehicleMake(string installationId, string? vehicleMake, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            Tariffs? item;
            if (string.IsNullOrWhiteSpace(vehicleMake))
            {
                item = null;
            }
            else
            {
                var make = vehicleMake.ToLowerInvariant();
                item = await context.Tariffs.FirstOrDefaultAsync(x => x.InstallationId == installationId && x.VehicleMakes != null && x.VehicleMakes.Contains(make));
            }

            if (item == null)
            {
                return await GetDefault(installationId, context);
            }

            return item;
        }

        private async Task<Tariffs?> GetDefault(string installationId, BControlDbContext context)
        {
            return await context.Tariffs.FirstOrDefaultAsync(x => x.InstallationId == installationId && x.IsDefault) ??
                   await context.Tariffs.FirstOrDefaultAsync(x => x.InstallationId == installationId); 
        }
    }
}
