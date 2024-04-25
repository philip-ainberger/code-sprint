using CodeSprint.Common.Dtos;
using CodeSprint.Common.Options;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace CodeSprint.Api.Services;

public class GitHubOAuthService : IGitHubOAuthService
{
    private readonly GithubOAuthOptions _options;
    private readonly IHttpClientFactory _httpClientFactory;

    public GitHubOAuthService(IOptions<GithubOAuthOptions> options, IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _httpClientFactory = httpClientFactory;
    }

    public Uri GetGitHubClientAuthorizationUri()
    {
        var queryParams = new Dictionary<string, string>()
        {
            { "client_id", _options.ClientId },
            { "scope", "user:email" }
        };

        var uri = new QueryBuilder(queryParams);

        return new Uri(new Uri(_options.OAuthClientAuthorizationEndpoint), uri.ToQueryString().Value!);
    }

    public async Task<string> GetBearerTokenAsync(string authorizationCode, string clientId, string clientSecret)
    {
        using (var httpClient = _httpClientFactory.CreateClient())
        {
            AddGitHubDefaultHeaders(httpClient);

            var requestBody = new
            {
                client_id = clientId,
                client_secret = clientSecret,
                code = authorizationCode,
                grant_type = "authorization_code"
            };

            var response = await httpClient.PostAsJsonAsync(_options.OAuthAccessTokenEndpoint, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Failed to retrieve access token!");
            }

            var content = await response.Content.ReadFromJsonAsync<GitHubTokenResponseDto>();

            if (content == null)
            {
                throw new InvalidOperationException("[GitHub][OAuth] Invalid response body!");
            }

            return content.AccessToken;
        }
    }

    public async Task<GitHubUserInfoDto> GetUserInfoAsync(string bearerToken)
    {
        using (var httpClient = _httpClientFactory.CreateClient())
        {
            AddGitHubDefaultHeaders(httpClient);
            AddGitHubBearerToken(httpClient, bearerToken);

            var response = await httpClient.GetAsync(_options.UserApiEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("[GitHub][OAuth] Failed to retrieve user information!");
            }

            var content = await response.Content.ReadFromJsonAsync<GitHubUserInfoDto>();

            if (content == null)
            {
                throw new InvalidOperationException("[GitHub][OAuth] Invalid response body!");
            }

            return content;
        }
    }

    private static void AddGitHubDefaultHeaders(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("code-sprint-api/1.0"); // TODO
    }

    private static void AddGitHubBearerToken(HttpClient httpClient, string bearerToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
    }
}