using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.Occupation
{
    public interface IOccupationRepository
    {
        Task AddAsync(Occupations occupation, BControlDbContext context);
        Task UpdateAsync(Occupations occupation, BControlDbContext context);
        Task UpdateRangeAsync(List<Occupations> occupations, BControlDbContext context);
        Task<List<Occupations>?> GetOccupationsAvailable(DateTime date, string installationId, string zoneId, string? tariffId, BControlDbContext context);
        Task<List<Occupations>?> GetExistingOccupationsByDate(
            DateTime fromDate,
            DateTime toDate,
            string installationId,
            string zoneId,
            string? tariffId,
            BControlDbContext context);
        Task AddRangeAsync(List<Occupations> occupations, BControlDbContext context);
        Task<List<Occupations?>> GetByDate(DateTime date, BControlDbContext context);
        Task<List<Occupations>> GetOccupationsByDateAndInsId(DateTime date, string insId, BControlDbContext context);
        Task<List<Occupations>> GetOccupationsByFiltersAsync(List<Tuple<DateTime?, string, string, string?>> filters, BControlDbContext context);
        Task<List<Occupations>> GetCurrentOccupations(BControlDbContext context, List<Installations> installations);
    }
}
