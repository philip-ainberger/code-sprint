using CodeSprint.Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace CodeSprint.Tests.Tests;

public class SessionProviderServiceTests
{
    private readonly SessionProviderService _sessionProviderService;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly DefaultHttpContext _httpContext;

    public SessionProviderServiceTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContext = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(_httpContext);

        _sessionProviderService = new SessionProviderService(_httpContextAccessorMock.Object);
    }

    [Fact]
    public void GetCurrentSessionUserId_UserAuthenticated_ReturnsUserId()
    {
        // Arrange
        var expectedUserId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, expectedUserId.ToString())
        }));

        _httpContext.User = claimsPrincipal;

        // Act
        var result = _sessionProviderService.GetCurrentSessionUserId();

        // Assert
        result.Should().Be(expectedUserId);
    }

    [Fact]
    public void GetCurrentSessionUserId_UserNotAuthenticated_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());  // No claims set

        // Act & Assert
        _sessionProviderService
            .Invoking(s => s.GetCurrentSessionUserId())
            .Should().Throw<UnauthorizedAccessException>()
            .WithMessage("*");
    }

    [Fact]
    public void GetCurrentSessionUserId_NoHttpContext_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns((HttpContext)null!);

        // Act & Assert
        _sessionProviderService
            .Invoking(s => s.GetCurrentSessionUserId())
            .Should().Throw<UnauthorizedAccessException>()
            .WithMessage("*");
    }
}