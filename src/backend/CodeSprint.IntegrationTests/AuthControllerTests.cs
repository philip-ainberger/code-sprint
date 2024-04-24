using CodeSprint.Common.Dtos;
using CodeSprint.Core.Models;
using FluentAssertions;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;

namespace CodeSprint.IntegrationTests;

public class AuthControllerTests : BaseApplicationTests
{
    private readonly HttpClient _httpClient;
    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<RefreshToken> _refreshTokenCollection;

    public AuthControllerTests(ApiWebApplicationFactory<Program> factory) : base(factory)
    {
        _httpClient = factory.CreateDefaultClient();
        _userCollection = _mongoCollectionProvider.GetCollection<User>("users");
        _refreshTokenCollection = _mongoCollectionProvider.GetCollection<RefreshToken>("refresh_tokens");
    }

    [Fact]
    public async Task AuthCallback_SuccessfulOAuthLogin_RefreshTokenInCookie()
    {
        // Act
        var loginResult = await _httpClient.GetAsync("/api/auth/callback?code=test");

        // Assert
        AssertHttpStatus(loginResult, HttpStatusCode.Found);
        AssertHeaderForCookie(loginResult);
        AssertSingleUserAndToken();
    }

    [Fact]
    public async Task AuthCallback_OverrideExistingRefreshToken_TokenOverriden()
    {
        // Arrange
        await _httpClient.GetAsync("/api/auth/callback?code=test");
        var previousUpdated = _refreshTokenCollection.AsQueryable().First().UpdatedAt;

        // Act
        var loginResult = await _httpClient.GetAsync("/api/auth/callback?code=test");

        // Arrange
        AssertHttpStatus(loginResult, HttpStatusCode.Found);
        AssertTokenRefreshed(previousUpdated);
    }

    [Fact]
    public async Task AuthCallback_InvalidCallbackCode_BadRequest()
    {
        AssertHttpStatus(await _httpClient.GetAsync("/api/auth/callback"), HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshTokenAsync_InvalidRefreshToken_ReturnsUnauthorized()
    {
        // Arrange
        AddCookieToHttpClient("invalidRefreshToken");

        // Act
        AssertHttpStatus(await _httpClient.GetAsync("/api/auth/refresh"), HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshTokenAsync_MissingRefreshToken_ReturnsUnauthorized()
    {
        AssertHttpStatus(await _httpClient.GetAsync("/api/auth/refresh"), HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshTokenAsync_OverrideExistingRefreshToken_ReturnAccessToken()
    {
        // Arrange
        await _httpClient.GetAsync("/api/auth/callback?code=test");
        var refreshToken = _refreshTokenCollection.AsQueryable().First().Token;
        AddCookieToHttpClient(refreshToken);

        // Act
        var response = await _httpClient.GetAsync("/api/auth/refresh");

        // Assert
        AssertHttpStatus(response, HttpStatusCode.OK);
        AssertRefreshTokenUpdated(response, refreshToken);
    }

    [Fact]
    public async Task ValidateTokenAsync_MissingToken_ReturnsUnauthorized()
    {
        AssertHttpStatus(await _httpClient.GetAsync("/api/auth/validate"), HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidateTokenAsync_UseRefreshTokenAsAccess_ReturnsUnauthorized()
    {
        // Arrange
        await _httpClient.GetAsync("/api/auth/callback?code=test");
        var refreshToken = _refreshTokenCollection.AsQueryable().First().Token;
        AddAuthorizationHeader(refreshToken);

        // Act & Assert
        AssertHttpStatus(await _httpClient.GetAsync("/api/auth/validate"), HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidateTokenAsync_UseAccessToken_ReturnsSameValidToken()
    {
        // Arrange
        await _httpClient.GetAsync("/api/auth/callback?code=test");
        var refreshToken = _refreshTokenCollection.AsQueryable().First().Token;
        AddCookieToHttpClient(refreshToken);

        var tokenResponse = await _httpClient.GetFromJsonAsync<JwtDto>("/api/auth/refresh");
        AddAuthorizationHeader(tokenResponse!.Token);

        // Act & Assert
        var response = await _httpClient.GetAsync("/api/auth/validate");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JwtDto>();
        body!.Token.Should().Be(tokenResponse.Token);
        body!.ExpiresIn.Should().BeLessThan(tokenResponse.ExpiresIn);
    }

    private static void AssertHttpStatus(HttpResponseMessage response, HttpStatusCode expectedStatus)
    {
        response.StatusCode.Should().Be(expectedStatus);
    }

    private static void AssertHeaderForCookie(HttpResponseMessage response)
    {
        response.Headers.Should().Contain(c => c.Key == "Set-Cookie" && c.Value.Count() == 2);
        var jwt = ExtractJwtFromCookie(response.Headers.First(c => c.Key == "Set-Cookie").Value.Last());
        jwt.Should().NotBeNullOrEmpty();
        IsJwt(jwt).Should().BeTrue();
    }

    private void AssertSingleUserAndToken()
    {
        _userCollection.AsQueryable().Count().Should().Be(1);
        _refreshTokenCollection.AsQueryable().Count().Should().Be(1);
    }

    private void AssertTokenRefreshed(DateTime previousUpdated)
    {
        var refreshTokens = _refreshTokenCollection.AsQueryable().ToList();
        refreshTokens.Should().HaveCount(1);
        refreshTokens[0].UpdatedAt.Should().BeAfter(previousUpdated);
    }

    private void AssertRefreshTokenUpdated(HttpResponseMessage response, string previousToken)
    {
        var body = response.Content.ReadFromJsonAsync<JwtDto>().Result;
        body.Should().NotBeNull();
        body!.Token.Should().NotBeNullOrEmpty();
        body!.ExpiresIn.Should().BeGreaterThan(0);

        var newToken = ExtractJwtFromCookie(response.Headers.First(c => c.Key == "Set-Cookie").Value.Last());
        newToken.Should().NotBeSameAs(previousToken);

        _refreshTokenCollection.AsQueryable().First().Token.Should().NotBeSameAs(previousToken, "Should be a new refresh token");
    }

    private void AddCookieToHttpClient(string token)
    {
        _httpClient.DefaultRequestHeaders.Add("Cookie", $"jwt_token={token}");
    }

    private void AddAuthorizationHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
    }

    private static string ExtractJwtFromCookie(string cookie)
    {
        var cookieParts = cookie.Split(';').FirstOrDefault()?.Split('=');
        return cookieParts!.Length > 1 ? cookieParts[1] : string.Empty;
    }

    private static bool IsJwt(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        return jwtTokenHandler.CanReadToken(token);
    }
}