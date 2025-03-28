using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlinkayOccupation.Domain.Repositories.Stay
{
    public class StaysRepository : IStaysRepository
    {
        public async Task AddAsync(Stays stay, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (stay == null) throw new ArgumentException("Stay Object can not be null.", nameof(stay));

            await context.Stays.AddAsync(stay);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Stays stay, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (stay == null) throw new ArgumentException("Stay Object can not be null.", nameof(stay));

            context.Stays.Update(stay);
            await context.SaveChangesAsync();
        }

        public async Task<Stays?> GetByIdAsync(string id, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id cannot be null.", nameof(id));

            return await context.Stays
                .Include(s => s.Installation)
                .Include(s => s.StaysParkingRights)
                    .ThenInclude(spr => spr.ParkingRight)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Stays>> GetStaysToProcessPaymentsAsync(BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var stays = await context.Stays
                        .Include(s => s.Installation)
                        .Include(s => s.StaysParkingRights)
                            .ThenInclude(spr => spr.ParkingRight)
                            .ThenInclude(t => t.Tariff)
                        .Where(s => s.EndPaymentProcessed != true && s.EndPaymentDate.HasValue)
                        .ToListAsync();

            //return stays.Where(s => s.EndPaymentDate.Value > s.Installation.DateTimeNow()).ToList();
            return stays.Where(s => s.InitPaymentDate.Value.Date == s.Installation.DateTimeNow().Date || s.EndPaymentDate.Value.Date == s.Installation.DateTimeNow().Date).ToList();
        }
    }
}
