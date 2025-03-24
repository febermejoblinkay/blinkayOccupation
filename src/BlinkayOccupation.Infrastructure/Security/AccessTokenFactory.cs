using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlinkayOccupation.Infrastructure.Security
{
    public class AccessTokenFactory : IAccessTokenFactory
    {
        private readonly string _issuer;
        private readonly SigningCredentials _signingCredentials;

        public AccessTokenFactory(string key, string issuer)
        {
            _issuer = issuer;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }

        public ClaimsIdentity CreateIdentity(string subject)
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, subject));
            return identity;
        }

        public AccessToken CreateToken(ClaimsIdentity identity, TimeSpan expires)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.Add(expires),
                Issuer = _issuer,
                Audience = _issuer,
                SigningCredentials = _signingCredentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new AccessToken(tokenHandler.WriteToken(token), token.ValidTo);
        }
    }

    public class AccessToken
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }

        public AccessToken(string token, DateTime expires)
        {
            Token = token;
            Expires = expires;
        }
    }
}
