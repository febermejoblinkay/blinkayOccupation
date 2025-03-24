using BlinkayOccupation.Domain.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace BlinkayOccupation.Domain.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IDbContextFactory<BControlDbContext> _contextFactory;
        private readonly ILogger<UnitOfWork> _logger;
        private BControlDbContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWork(IDbContextFactory<BControlDbContext> contextFactory, ILogger<UnitOfWork> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public BControlDbContext Context
        {
            get
            {
                if (_context == null)
                    _context = _contextFactory.CreateDbContext();
                return _context;
            }
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                throw new InvalidOperationException("A transaction is already in progress.");

            _transaction = await Context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction in progress to commit.");

            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction in progress to rollback.");

            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
