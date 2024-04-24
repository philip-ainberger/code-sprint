using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CodeSprint.Common;

public static class Defaults
{
    public static TokenValidationParameters GetDefaultTokenValidationParameters(string key, string issuer, string audience)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidIssuer = issuer,
            ValidAudience = audience
        };
    }
}