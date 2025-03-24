using BlinkayOccupation.Domain.Contexts;
using BlinkayOccupation.Domain.Repositories.User;
using Microsoft.EntityFrameworkCore;

namespace BlinkayOccupation.Domain.Repositories.Users
{
    public class UsersRepository : IUsersRepository
    {
        public async Task<Models.Users?> GetByIdAsync(string id, BControlDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("The Id cannot be null or empty.", nameof(id));

            return await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
