using BlinkayOccupation.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BlinkayOccupation.Domain.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            SetAuditFields(eventData.Context);
            return result;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            SetAuditFields(eventData.Context);
            return new ValueTask<InterceptionResult<int>>(result);
        }

        private void SetAuditFields(DbContext? context)
        {
            if (context == null) return;

            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added && entry.Entity is IAuditable auditable)
                {
                    entry.Property("Created").CurrentValue = now;
                    entry.Property("Deleted").CurrentValue = false;
                }

                if (entry.State == EntityState.Modified && entry.Entity is IAuditable)
                {
                    entry.Property("Updated").CurrentValue = now;
                }
            }
        }
    }
}
