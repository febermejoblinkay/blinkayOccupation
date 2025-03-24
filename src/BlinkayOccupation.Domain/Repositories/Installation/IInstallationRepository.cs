using BlinkayOccupation.Domain.Contexts;

namespace BlinkayOccupation.Domain.Repositories.Installation
{
    public interface IInstallationRepository
    {
        Task<Models.Installations> GetByIdAsync(string id, BControlDbContext context);
    }
}
