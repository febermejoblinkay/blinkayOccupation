using BlinkayOccupation.Domain.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlinkayOccupation.Domain.Repositories.Installation
{
    public class InstallationRepository : IInstallationRepository
    {
        public async Task<Models.Installations> GetByIdAsync(string id, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id can not be null or empty.", nameof(id));

            return await context.Installations.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Models.Installations>> GetAllAsync(BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            return await context.Installations.ToListAsync();
        }
    }
}
