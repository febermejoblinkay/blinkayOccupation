using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlinkayOccupation.Domain.Repositories.Occupation
{
    public class OccupationRepository : IOccupationRepository
    {
        public async Task AddAsync(Occupations occupation, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (occupation == null) throw new ArgumentException("Occupation Object can not be null.", nameof(occupation));

            await context.Occupations.AddAsync(occupation);
            //await context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<Occupations> occupations, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (occupations?.Count == 0) throw new ArgumentException("Occupations list can not be null.", nameof(occupations));

            context.Occupations.AddRangeAsync(occupations);
            //await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Occupations occupation, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (occupation == null) throw new ArgumentException("Occupation Object can not be null.", nameof(occupation));

            context.Occupations.Update(occupation);
            //await context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(List<Occupations> occupations, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (occupations?.Count == 0) throw new ArgumentException("Occupations list can not be null.", nameof(occupations));

            context.Occupations.UpdateRange(occupations);
            //await context.SaveChangesAsync();
        }

        public async Task<List<Occupations>?> GetOccupationsAvailable(
            DateTime date,
            string installationId,
            string zoneId,
            string? tariffId,
            BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (date == default(DateTime) ||
                string.IsNullOrWhiteSpace(installationId) ||
                string.IsNullOrWhiteSpace(zoneId))
            {
                throw new ArgumentException("Parameters are null or empty.");
            }

            return await context.Occupations.Where(o =>
                o.Date.Value.Date == date.Date &&
                o.InstallationId == installationId &&
                o.ZoneId == zoneId &&
                !o.Deleted &&
                (o.TariffId == tariffId || o.TariffId == null)
            ).ToListAsync();
        }

        public async Task<List<Occupations>?> GetExistingOccupationsByDate(
            DateTime fromDate,
            DateTime toDate,
            string installationId,
            string zoneId,
            string? tariffId,
            BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (fromDate == default ||
                toDate == default ||
                string.IsNullOrWhiteSpace(installationId) ||
                string.IsNullOrWhiteSpace(zoneId))
            {
                throw new ArgumentException("Parameters are null or empty.");
            }

            return await context.Occupations
                .Where(o =>
                    o.Date.Value.Date >= fromDate.Date &&
                    o.Date.Value.Date <= toDate.Date &&
                    o.InstallationId == installationId &&
                    o.ZoneId == zoneId &&
                    !o.Deleted &&
                    (o.TariffId == tariffId || o.TariffId == null))
                .ToListAsync();
        }

        public async Task<List<Occupations?>> GetByDate(DateTime date, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (date == default) throw new ArgumentException("Date can not be null.", nameof(date));

            return await context.Occupations
                .Where(o => o.Date.Value.Date >= date.Date)
                .ToListAsync();
        }

        public async Task<List<Occupations>> GetOccupationsByDateAndInsId(DateTime date, string insId, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (date == default || string.IsNullOrWhiteSpace(insId)) throw new ArgumentException("date or insId can not be null.", nameof(date));

            return await context.Occupations.Where(o => o.InstallationId == insId && o.Date == date.Date).ToListAsync();
        }

        public async Task<List<Occupations>> GetOccupationsByFiltersAsync(List<Tuple<DateTime?, string, string, string?>> filters, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (filters == null || !filters.Any())
            {
                return new List<Occupations>();
            }

            var query = context.Occupations.AsQueryable();

            var orPredicates = filters.Select<Tuple<DateTime?, string, string, string?>, Expression<Func<Occupations, bool>>>(f =>
                (Occupations s) =>
                    (f.Item1 == s.Date) &&
                    (f.Item2 == s.InstallationId) &&
                    (f.Item3 == s.ZoneId) &&
                    (f.Item4 == s.TariffId || f.Item4 == null)
            ).ToList();

            var finalPredicate = orPredicates.Aggregate((accumulatedPredicate, currentPredicate) =>
                accumulatedPredicate.Or(currentPredicate)
            );

            query = query.Where(finalPredicate);

            return await query.ToListAsync();
        }
    }

    public static class PredicateExtensions
    {
        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}
