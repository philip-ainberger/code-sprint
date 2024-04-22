using CodeSprint.Common.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CodeSprint.Common.Jwt;

public class JwtService : IJwtService
{
    private JwtOptions _options;

    public JwtService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public JwtInfo BuildJwt(Guid userId)
    {
        return BuildJwt(userId, DateTime.UtcNow.AddMinutes(15));
    }

    public JwtInfo BuildRefreshJwt(Guid userId)
    {
        return BuildJwt(userId, DateTime.UtcNow.AddDays(2));
    }

    private JwtInfo BuildJwt(Guid userId, DateTime expiration)
    {
        var signingKey = new SymmetricSecurityKey(GetKey());
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new List<Claim>() { claim }),
            Expires = expiration,
            SigningCredentials = credentials,
            Issuer = _options.ValidIssuer,
            Audience = _options.ValidAudience
        };

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var securityToken = jwtTokenHandler.CreateToken(securityTokenDescriptor);
        var tokenString = jwtTokenHandler.WriteToken(securityToken);

        return new JwtInfo(tokenString, expiration);
    }

    private byte[] GetKey() => Encoding.UTF8.GetBytes(_options.Key);
}