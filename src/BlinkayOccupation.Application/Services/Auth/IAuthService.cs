using BlinkayOccupation.Application.Models;
using BlinkayOccupation.Infrastructure.Security;

namespace BlinkayOccupation.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<AccessToken?> LoginAsync(LoginRequest request);
    }
}
