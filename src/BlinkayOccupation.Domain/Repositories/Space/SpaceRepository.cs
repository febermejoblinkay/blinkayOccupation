using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlinkayOccupation.Domain.Repositories.Space
{
    public class SpaceRepository : ISpaceRepository
    {
        public async Task<Spaces?> GetByIdAsync(string id, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id cannot be null.", nameof(id));

            return await context.Spaces.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
