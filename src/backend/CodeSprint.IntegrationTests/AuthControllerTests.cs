using CodeSprint.Common.Dtos;
using CodeSprint.Core.Models;
using FluentAssertions;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;

namespace CodeSprint.IntegrationTests;

public class AuthControllerTests : BaseApplicationTest
{
    public AuthControllerTests(ApiWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task AuthCallback_SuccessfulOAuthLogin_RefreshTokenInCookie()
    {
        // Arrange
        var userCollection = _mongoCollectionProvider.GetCollection<User>("users");
        var refreshTokenCollection = _mongoCollectionProvider.GetCollection<RefreshToken>("refresh_tokens");
        var httpClient = _factory.CreateDefaultClient();

        // Act
        var loginResult = await httpClient.GetAsync("/api/auth/callback?code=test");

        // Assert
        loginResult.StatusCode.Should().Be(HttpStatusCode.Found);
        
        // should contain two header values. first for deletion and second for setting the cookie
        loginResult.Headers.Should().Contain(c => c.Key == "Set-Cookie" && c.Value.Count() == 2);
        var rawHeaderValue = loginResult.Headers.First(c => c.Key == "Set-Cookie").Value.Last();
        var jwt = ExtractJwtFromCookie(rawHeaderValue);

        jwt.Should().NotBeNullOrEmpty();
        IsJwt(jwt).Should().BeTrue();

        var userCount = userCollection.AsQueryable().Count();
        userCount.Should().Be(1);
        
        var refreshTokenCount = refreshTokenCollection.AsQueryable().Count();
        refreshTokenCount.Should().Be(1);
    }

    [Fact]
    public async Task AuthCallback_OverrideExistingRefreshToken_TokenOverriden()
    {
        // Arrange
        var userCollection = _mongoCollectionProvider.GetCollection<User>("users");
        var refreshTokenCollection = _mongoCollectionProvider.GetCollection<RefreshToken>("refresh_tokens");
        var httpClient = _factory.CreateDefaultClient();

        // Act
        await httpClient.GetAsync("/api/auth/callback?code=test");
        var previousUpdated = refreshTokenCollection.AsQueryable().First().UpdatedAt;

        var loginResult = await httpClient.GetAsync("/api/auth/callback?code=test");

        // Assert
        loginResult.StatusCode.Should().Be(HttpStatusCode.Found);

        var userCount = userCollection.AsQueryable().Count();
        userCount.Should().Be(1);

        var refreshTokens = refreshTokenCollection.AsQueryable().ToList();
        refreshTokens.Should().HaveCount(1);
        refreshTokens[0].UpdatedAt.Should().BeAfter(previousUpdated);

        var rawHeaderValue = loginResult.Headers.First(c => c.Key == "Set-Cookie").Value.Last();
        var jwt = ExtractJwtFromCookie(rawHeaderValue);
        refreshTokens[0].Token.Should().Be(jwt);
    }

    [Fact]
    public async Task AuthCallback_InvalidCallbackCode_BadRequest()
    {
        // Act
        var httpClient = _factory.CreateDefaultClient();
        var loginResult = await httpClient.GetAsync("/api/auth/callback");

        // Assert
        loginResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshTokenAsync_InvalidRefreshToken_ReturnsUnauthorized()
    {
        // Arrange
        var httpClient = _factory.CreateDefaultClient();
        var invalidRefreshToken = "invalidRefreshToken";
        httpClient.DefaultRequestHeaders.Add("Cookie", $"jwt_token={invalidRefreshToken}");

        // Act
        var response = await httpClient.GetAsync("/api/auth/refresh");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshTokenAsync_MissingRefreshToken_ReturnsUnauthorized()
    {
        // Arrange
        var httpClient = _factory.CreateDefaultClient();

        // Act
        var response = await httpClient.GetAsync("/api/auth/refresh");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshTokenAsync_OverrideExistingRefreshToken_ReturnAccessToken()
    {
        // Arrange
        var refreshTokenCollection = _mongoCollectionProvider.GetCollection<RefreshToken>("refresh_tokens");
        var httpClient = _factory.CreateDefaultClient();

        await httpClient.GetAsync("/api/auth/callback?code=test");
        var refreshToken = refreshTokenCollection.AsQueryable().First().Token;

        httpClient = _factory.CreateDefaultClient();
        httpClient.DefaultRequestHeaders.Add("Cookie", $"jwt_token={refreshToken}");

        // Act
        var response = await httpClient.GetAsync("/api/auth/refresh");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JwtDto>();
        body.Should().NotBeNull();
        body!.Token.Should().NotBeNullOrEmpty();
        body!.ExpiresIn.Should().BeGreaterThan(1);

        var rawHeaderValue = response.Headers.First(c => c.Key == "Set-Cookie").Value.Last();
        var jwt = ExtractJwtFromCookie(rawHeaderValue);

        jwt.Should().NotBeSameAs(refreshToken);
        refreshTokenCollection.AsQueryable().First().Token.Should().NotBeSameAs(refreshToken, "Should be a new refresh token");
    }

    [Fact]
    public async Task ValidateTokenAsync_MissingToken_ReturnsUnauthorized()
    {
        // Arrange
        var httpClient = _factory.CreateDefaultClient();

        // Act
        var response = await httpClient.GetAsync("/api/auth/validate");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    
    [Fact]
    public async Task ValidateTokenAsync_UseRefreshTokenAsAccess_ReturnsUnauthorized()
    {
        // Arrange
        var refreshTokenCollection = _mongoCollectionProvider.GetCollection<RefreshToken>("refresh_tokens");
        var httpClient = _factory.CreateDefaultClient();

        await httpClient.GetAsync("/api/auth/callback?code=test");
        var refreshToken = refreshTokenCollection.AsQueryable().First().Token;

        httpClient = _factory.CreateDefaultClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + refreshToken);

        // Act
        var response = await httpClient.GetAsync("/api/auth/validate");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private string ExtractJwtFromCookie(string cookie)
    {
        var cookieParts = cookie.Split(';').FirstOrDefault()?.Split('=');
        return cookieParts.Length > 1 ? cookieParts[1] : string.Empty;
    }

    private bool IsJwt(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        return jwtTokenHandler.CanReadToken(token);
    }
}