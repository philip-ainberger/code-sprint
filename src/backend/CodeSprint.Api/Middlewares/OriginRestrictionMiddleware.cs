using CodeSprint.Common.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace CodeSprint.Api.Middlewares;

public class OriginRestrictionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _allowedOrigin;

    public OriginRestrictionMiddleware(RequestDelegate next, IOptions<ApplicationOptions> options)
    {
        _next = next;
        _allowedOrigin = options.Value.HostedClientUrl;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Origin", out StringValues origin)
            && !origin.Contains(_allowedOrigin, StringComparer.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = 403; // Forbidden
            await context.Response.WriteAsync("Origin not allowed");
            return;
        }

        await _next(context);
    }
}