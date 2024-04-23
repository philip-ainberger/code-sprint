using Microsoft.AspNetCore.Http;
using static Google.Rpc.Context.AttributeContext.Types;

namespace CodeSprint.Common.Extensions;

public static class CookieExtensions
{
    public static readonly string CookieKey = "jwt_token";

    public static CookieOptions DefaultCookieOptions => new()
    {
        HttpOnly = true,
        Secure = true,
        Expires = DateTimeOffset.UtcNow.AddHours(1)
    };

    public static string GetRefreshTokenFromCookie(this HttpContext httpContext)
    {
        return httpContext.Request.Cookies[CookieKey];
    }

    public static void AddRefreshTokenToCookie(this HttpContext httpContext, string jwt)
    {
        httpContext.Response.Cookies.Delete(CookieKey);
        httpContext.Response.Cookies.Append(CookieKey, jwt, DefaultCookieOptions);
    }
}
