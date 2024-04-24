using CodeSprint.Common.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CodeSprint.Common.Jwt;

public class JwtService : IJwtService
{
    private readonly JwtOptions _options;

    public JwtService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public JwtSecurityToken ReadTokenAsync(string jwt)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        return jwtTokenHandler.ReadJwtToken(jwt);
    }

    public async Task<TokenValidationResult> ValidateRefreshTokenAsync(string jwt)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var validationResult = await jwtTokenHandler.ValidateTokenAsync(jwt,
            Defaults.GetDefaultTokenValidationParameters(_options.RefreshTokensKey, _options.ValidIssuer, _options.ValidAudience));

        return validationResult;
    }

    public JwtInfo BuildJwt(Guid userId)
    {
        return BuildJwt(userId, DateTime.UtcNow.AddMinutes(15), GetAccessTokenKey());
    }

    public JwtInfo BuildRefreshJwt(Guid userId)
    {
        return BuildJwt(userId, DateTime.UtcNow.AddDays(2), GetRefreshTokenKey());
    }

    private JwtInfo BuildJwt(Guid userId, DateTime expiration, byte[] key)
    {
        var signingKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claim = new Claim(JwtRegisteredClaimNames.Sub, userId.ToString());

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

    private byte[] GetAccessTokenKey() => Encoding.UTF8.GetBytes(_options.AccessTokensKey);

    private byte[] GetRefreshTokenKey() => Encoding.UTF8.GetBytes(_options.RefreshTokensKey);
}