using CodeSprint.Api.Services;
using CodeSprint.Common.Dtos;
using CodeSprint.Common.Extensions;
using CodeSprint.Common.Jwt;
using CodeSprint.Common.Options;
using CodeSprint.Core.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace CodeSprint.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly GitHubOAuthOptions _gitHubOptions;
    private readonly ApplicationOptions _applicationOptions;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IGitHubOAuthService _gitHubOAuthService;
    private readonly IJwtService _jwtService;

    public AuthController(
        IOptions<GitHubOAuthOptions> gitHubOptions,
        IOptions<ApplicationOptions> applicationOptions,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IGitHubOAuthService gitHubOAuthService,
        IJwtService jwtBuilder)
    {
        _gitHubOptions = gitHubOptions.Value;
        _applicationOptions = applicationOptions.Value;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _gitHubOAuthService = gitHubOAuthService;
        _jwtService = jwtBuilder;
    }

    [HttpGet("github-auth")]
    public IActionResult RedirectToGitHubClientAuthorization()
    {
        return Redirect(_gitHubOAuthService.GetGitHubClientAuthorizationUri().ToString());
    }

    [HttpGet("callback")]
    public async Task<IActionResult> GitHubCallback([FromQuery] string? code)
    {
        if (string.IsNullOrEmpty(code)) return BadRequest("Authorization code missing");

        var accessToken = await _gitHubOAuthService.GetBearerTokenAsync(code, _gitHubOptions.ClientId, _gitHubOptions.ClientSecret);
        if (string.IsNullOrEmpty(accessToken)) return BadRequest("Failed to retrieve access token");

        var userInfo = await _gitHubOAuthService.GetUserInfoAsync(accessToken);
        var userId = await TryFindOrAddUserAsync(userInfo);

        var jwtToken = _jwtService.BuildRefreshJwt(userId);
        await _refreshTokenRepository.AddOrOverrideAsync(userId, jwtToken.TokenValue);

        HttpContext.AddRefreshTokenToCookie(jwtToken.TokenValue);
        
        return Redirect(_applicationOptions.HostedClientUrl);
    }

    [Authorize]
    [HttpGet("validate")]
    public async Task<IActionResult> ValidateTokenAsync()
    {
        var jwt = await HttpContext.GetTokenAsync("access_token");

        if (string.IsNullOrEmpty(jwt)) return Unauthorized("Access token is missing");

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var token = jwtTokenHandler.ReadJwtToken(jwt);

        return Ok(new JwtDto() { Token = jwt, ExpiresIn = token.ValidTo.Subtract(DateTime.UtcNow).Milliseconds });
    }

    [HttpGet("refresh")]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        var refreshToken = HttpContext.GetRefreshTokenFromCookie();
        if (string.IsNullOrEmpty(refreshToken)) return Unauthorized("Refresh token is missing");

        var validationResult = await _jwtService.ValidateRefreshTokenAsync(refreshToken);
        if (!validationResult.IsValid) return Unauthorized("Invalid refresh token");

        var token = _jwtService.ReadTokenAsync(refreshToken);

        if (!Guid.TryParse(token.Subject, out Guid userId)) return Unauthorized("Token contains invalid subject");

        var entityToken = await _refreshTokenRepository.GetByUserIdAsync(userId);
        if (entityToken.Token != refreshToken) return Unauthorized("Invalid refresh token");

        var newJwtToken = _jwtService.BuildJwt(userId);
        var newRefreshToken = _jwtService.BuildRefreshJwt(userId);

        await _refreshTokenRepository.AddOrOverrideAsync(userId, newRefreshToken.TokenValue);

        HttpContext.AddRefreshTokenToCookie(newRefreshToken.TokenValue);

        return Ok(new JwtDto() { Token = newJwtToken.TokenValue, ExpiresIn = newJwtToken.Expiration.Subtract(DateTime.UtcNow).Milliseconds });
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
