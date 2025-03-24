using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Domain.Repositories.Stay
{
    public interface IStaysRepository
    {
        Task AddAsync(Stays stay, BControlDbContext context);
        Task UpdateAsync(Stays stay, BControlDbContext context);
        Task<Stays> GetByIdAsync(string id, BControlDbContext context);
    }
}
