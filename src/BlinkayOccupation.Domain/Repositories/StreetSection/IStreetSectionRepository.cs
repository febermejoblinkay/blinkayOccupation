using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.StreetSection
{
    public interface IStreetSectionRepository
    {
        Task<StreetSections?> GetByIdAsync(string id, BControlDbContext context);
    }
}
