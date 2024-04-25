using CodeSprint.Api.Services;
using CodeSprint.Common.Dtos;
using CodeSprint.Common.Options;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace CodeSprint.Tests.Tests;

public class GitHubOAuthServiceTests
{
    private readonly GitHubOAuthService _gitHubOAuthService;
    private readonly Mock<IOptions<GithubOAuthOptions>> _optionsMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly GithubOAuthOptions _defaultOptions = new()
    {
        ClientId = "clientId",
        ClientSecret = "clientSecret",
        OAuthAccessTokenEndpoint = "https://api.github.com/oauth/access_token",
        OAuthClientAuthorizationEndpoint = "https://github.com/login/oauth/authorize",
        UserApiEndpoint = "https://api.github.com/user"
    };

    public GitHubOAuthServiceTests()
    {
        _optionsMock = new Mock<IOptions<GithubOAuthOptions>>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        _optionsMock.Setup(o => o.Value).Returns(_defaultOptions);

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        _gitHubOAuthService = new GitHubOAuthService(_optionsMock.Object, _httpClientFactoryMock.Object);
    }

    [Fact]
    public async Task GetBearerTokenAsync_ValidCredentials_ReturnsAccessToken()
    {
        // Arrange
        var authorizationCode = "code123";
        var expectedResult = new GitHubTokenResponseDto() { AccessToken = "access_token123", TokenType = "Bearer" };

        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(expectedResult))
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _gitHubOAuthService.GetBearerTokenAsync(authorizationCode, _defaultOptions.ClientId, _defaultOptions.ClientSecret);

        // Assert
        result.Should().Be(expectedResult.AccessToken);
    }

    [Fact]
    public async Task GetUserInfoAsync_ValidToken_ReturnsUserInfo()
    {
        // Arrange
        var bearerToken = "valid_token";
        var expectedResult = new GitHubUserInfoDto()
        {
            Id = 1,
            Name = "TestUser",
            NodeId = "1",
            AvatarUrl = "https://domain.com/image.jpg",
            Email = "test@user.com",
            Login = "username"
        };

        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(expectedResult))
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _gitHubOAuthService.GetUserInfoAsync(bearerToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedResult.Id);
        result.Name.Should().Be(expectedResult.Name);
        result.NodeId.Should().Be(expectedResult.NodeId);
        result.AvatarUrl.Should().Be(expectedResult.AvatarUrl);
        result.Email.Should().Be(expectedResult.Email);
        result.Login.Should().Be(expectedResult.Login);
    }

    [Fact]
    public async Task GetBearerTokenAsync_InvalidResponse_ThrowsInvalidOperationException()
    {
        // Arrange
        var authorizationCode = "invalid_code";

        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act & Assert
        await _gitHubOAuthService
            .Invoking(y => y.GetBearerTokenAsync(authorizationCode, _defaultOptions.ClientId, _defaultOptions.ClientSecret))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to retrieve access token!");
    }

    [Fact]
    public void GetGitHubClientAuthorizationUri_ValidOptionValues_BuildCorrectUri()
    {
        // Act
        var uri = _gitHubOAuthService.GetGitHubClientAuthorizationUri();

        // Assert
        uri.Should().Be($"{_defaultOptions.OAuthClientAuthorizationEndpoint}?client_id={_defaultOptions.ClientId}&scope=user%3Aemail");
    }
}