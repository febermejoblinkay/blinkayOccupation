using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace BlinkayOccupation.Domain.Repositories.OccupationSnapshot
{
    public class OccupationSnapshotRepository : IOccupationSnapshotRepository
    {
        public async Task AddRangeAsync(List<OccupationsSnapshots> snapshots, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (snapshots?.Count == 0) throw new ArgumentException("snapshots list can not be null.", nameof(snapshots));

            context.OccupationsSnapshots.AddRangeAsync(snapshots);
        }

        public async Task UpdateRangeAsync(List<OccupationsSnapshots> snapshots, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (snapshots?.Count == 0) throw new ArgumentException("snapshots list can not be null.", nameof(snapshots));

            context.OccupationsSnapshots.UpdateRange(snapshots);
        }
    }
}
