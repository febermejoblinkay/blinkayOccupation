using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.Occupation
{
    public interface IOccupationRepository
    {
        Task AddAsync(Occupations occupation, BControlDbContext context);
        Task UpdateAsync(Occupations occupation, BControlDbContext context);
        Task<List<Occupations>?> GetOccupationsAvailable(DateTime date, string installationId, string zoneId, string? tariffId, BControlDbContext context);
    }
}
