using CodeSprint.Common.Options;
using CodeSprint.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CodeSprint.Common.Extensions;
using CodeSprint.Common.Dtos;
using CodeSprint.Common.Jwt;

namespace CodeSprint.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IOptions<GithubOAuthOptions> _gitHubOptions;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IJwtService _jwtBuilder;

    public AuthController(
        IOptions<GithubOAuthOptions> gitHubOptions,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IHttpClientFactory httpClientFactory,
        IJwtService jwtBuilder)
    {
        _gitHubOptions = gitHubOptions;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _httpClientFactory = httpClientFactory;
        _jwtBuilder = jwtBuilder;
    }

    [HttpGet("callback")]
    public async Task<IActionResult> GitHubCallback([FromQuery] string? code)
    {
        if (string.IsNullOrEmpty(code)) return BadRequest("Authorization code missing");

        var accessToken = await GetAccessTokenAsync(code);
        if (string.IsNullOrEmpty(accessToken)) return BadRequest("Failed to retrieve access token");

        var userInfo = await GetUserInfoAsync(accessToken);
        var userId = await TryFindOrAddUserAsync(userInfo);

        var jwtToken = _jwtBuilder.BuildRefreshJwt(userId);
        await _refreshTokenRepository.AddOrOverrideAsync(userId, jwtToken.TokenValue);

        HttpContext.AddRefreshTokenToCookie(jwtToken.TokenValue);
        return Redirect($"http://localhost:4200/");
    }

    private async Task<string> GetAccessTokenAsync(string authorizationCode)
    {
        using (var client = _httpClientFactory.CreateClient())
        {
            return await client.GetBearerTokenAsync(authorizationCode, _gitHubOptions.Value.ClientId, _gitHubOptions.Value.ClientSecret);
        }
    }

    private async Task<GitHubUserInfoDto> GetUserInfoAsync(string bearerToken)
    {
        using (var client = _httpClientFactory.CreateClient())
        {
            return await client.GetUserInfoAsync(bearerToken);
        }
    }

    private async Task<Guid> TryFindOrAddUserAsync(GitHubUserInfoDto userInfo)
    {
        var user = await _userRepository.GetByExternalIdAsync(userInfo.Id.ToString());

        if (user == null)
        {
            user = await _userRepository.GetByEmailAsync(userInfo.Email);

            if (user == null)
            {
                var entity = userInfo.ToNewEntity();
                await _userRepository.AddAsync(entity);

                return entity.Id;
            }

            return user.Id;
        }

        return user.Id;
    }
}
