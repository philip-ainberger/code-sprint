using System.Security.Claims;

namespace CodeSprint.Api.Services;

public class SessionProviderService : ISessionProviderService
{
    private readonly IHttpContextAccessor _httpContext;

    public SessionProviderService(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }

    public Guid GetCurrentSessionUserId()
    {
        var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();

        return Guid.Parse(userId);
    }
}