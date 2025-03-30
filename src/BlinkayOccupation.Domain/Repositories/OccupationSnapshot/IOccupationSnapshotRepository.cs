using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.OccupationSnapshot
{
    public interface IOccupationSnapshotRepository
    {
        Task AddRangeAsync(List<OccupationsSnapshots> snapshots, BControlDbContext context);
        Task UpdateRangeAsync(List<OccupationsSnapshots> snapshots, BControlDbContext context);
    }
}
