using Microsoft.AspNetCore.Http;

namespace CodeSprint.Common.Extensions;

public static class CookieExtensions
{
    public static CookieOptions DefaultCookieOptions => new()
    {
        HttpOnly = true,
        Secure = true
    };

    public static void AddRefreshTokenToCookie(this HttpContext httpContext, string jwt)
    {
        httpContext.Response.Cookies.Append("jwt_token", jwt, DefaultCookieOptions);
    }
}
