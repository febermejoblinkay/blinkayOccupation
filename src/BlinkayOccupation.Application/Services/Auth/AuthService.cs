using BlinkayOccupation.Application.Models;
using BlinkayOccupation.Domain.Repositories.User;
using BlinkayOccupation.Domain.UnitOfWork;
using BlinkayOccupation.Infrastructure.Security;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BlinkayOccupation.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IAccessTokenFactory _tokenFactory;

        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<AuthService> _logger;

        public AuthService(IUsersRepository userRepository, IAccessTokenFactory tokenFactory, IUnitOfWork unitOfWork, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _tokenFactory = tokenFactory;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AccessToken?> LoginAsync(LoginRequest request)
        {
            AccessToken? token = null;
            try
            {
                var user = await _userRepository.GetByIdAsync(request.Id, _unitOfWork.Context);
                if (user == null || !user.IsValidPassword(request.Password))
                    return null;

                var identity = _tokenFactory.CreateIdentity(user.Id.ToString());
                identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));

                token = _tokenFactory.CreateToken(identity, TimeSpan.FromHours(1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to Login user: {user}", request.Id);
            }
            finally
            {
                await _unitOfWork.Context.DisposeAsync();
            }

            return token;
        }
    }
}
