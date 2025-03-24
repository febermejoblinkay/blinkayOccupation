using System.Security.Claims;

namespace BlinkayOccupation.Infrastructure.Security
{
    public interface IAccessTokenFactory
    {
        AccessToken CreateToken(ClaimsIdentity identity, TimeSpan expires);
        ClaimsIdentity CreateIdentity(string subject);
    }
}
