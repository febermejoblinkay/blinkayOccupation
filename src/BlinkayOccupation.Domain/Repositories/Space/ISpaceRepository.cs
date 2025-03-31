using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.Space
{
    public interface ISpaceRepository
    {
        Task<Spaces?> GetByIdAsync(string id, BControlDbContext context);
    }
}
