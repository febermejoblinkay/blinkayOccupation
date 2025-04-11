using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.Tariff
{
    public interface ITariffRepository
    {
        Task<List<Tariffs>> GetTariffByInsId(string insId, BControlDbContext context);
        Task<Tariffs?> FindByVehicleMake(string installationId, string? vehicleMake, BControlDbContext context);
    }
}
