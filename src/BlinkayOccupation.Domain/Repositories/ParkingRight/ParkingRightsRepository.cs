using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlinkayOccupation.Domain.Repositories.ParkingRight
{
    public class ParkingRightsRepository : IParkingRightsRepository
    {
        public async Task AddAsync(ParkingRights pRight, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (pRight == null) throw new ArgumentException("pRight Object can not be null.", nameof(pRight));

            context.ParkingRights.Add(pRight);
            await context.SaveChangesAsync();
        }

        public async Task<List<Models.ParkingRights>?> GetByExternalIdsAsync(List<string> ids, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (ids.Count == 0) throw new ArgumentException("Ids cannot be null or empty.", nameof(ids));

            return await context.ParkingRights.Where(x => ids.Contains(x.ExternalId)).ToListAsync();
        }

        public async Task<Models.ParkingRights?> GetByValidPlateAsync(string plate, DateTime date, TimeSpan expandBy, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(plate)) throw new ArgumentException("Plate cannot be null or empty.", nameof(plate));

            var parkingRights = await context.ParkingRights.Include(x => x.Tariff).Where(x => x.Plates != null && x.Plates.Contains(plate) && x.ValidFrom - expandBy <= date && date < x.ValidTo.Value + expandBy).ToListAsync();

            switch (parkingRights.Count)
            {
                case 0:
                    return null;

                case 1:
                    return parkingRights.FirstOrDefault();
            }

            var inside = parkingRights.FirstOrDefault(x => x.ValidFrom <= date && date < x.ValidTo);
            if (inside is not null)
            {
                return inside;
            }
            else
            {
                return parkingRights.FirstOrDefault();
            }
        }
    }
}
