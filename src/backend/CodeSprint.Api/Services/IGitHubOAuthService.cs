using CodeSprint.Common.Dtos;

namespace CodeSprint.Api.Services;

public interface IGitHubOAuthService
{
    Task<string> GetBearerTokenAsync(string authorizationCode, string clientId, string clientSecret);
    Task<GitHubUserInfoDto> GetUserInfoAsync(string bearerToken);
}