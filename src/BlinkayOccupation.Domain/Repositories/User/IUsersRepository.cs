using BlinkayOccupation.Domain.Contexts;

namespace BlinkayOccupation.Domain.Repositories.User
{
    public interface IUsersRepository
    {
        Task<Models.Users?> GetByIdAsync(string id, BControlDbContext context);
    }
}
