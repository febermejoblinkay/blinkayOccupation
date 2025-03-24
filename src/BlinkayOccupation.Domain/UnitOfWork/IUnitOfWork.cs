using BlinkayOccupation.Domain.Contexts;

namespace BlinkayOccupation.Domain.UnitOfWork
{
    public interface IUnitOfWork
    {
        BControlDbContext Context { get; }
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task<int> SaveChangesAsync();
    }
}
