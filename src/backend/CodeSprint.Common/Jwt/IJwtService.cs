using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace CodeSprint.Common.Jwt;

public interface IJwtService
{
    JwtSecurityToken ReadTokenAsync(string jwt);
    Task<TokenValidationResult> ValidateRefreshTokenAsync(string jwt);
    JwtInfo BuildJwt(Guid userId);
    JwtInfo BuildRefreshJwt(Guid userId);
}