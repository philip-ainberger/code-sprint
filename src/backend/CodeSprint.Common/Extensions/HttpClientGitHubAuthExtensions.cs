using CodeSprint.Common.Dtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CodeSprint.Common.Extensions;

public static class HttpClientGitHubAuthExtensions
{
    private const string GITHUB_ACCESS_TOKEN_URL = "https://github.com/login/oauth/access_token";
    private const string GITHUB_USER_URL = "https://api.github.com/user";

    public static void AddGitHubDefaultHeaders(this HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("code-sprint-api/1.0"); // TODO
    }

    public static void AddGitHubBearerToken(this HttpClient httpClient, string bearerToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
    }

    public static async Task<GitHubUserInfoDto> GetUserInfoAsync(this HttpClient httpClient, string bearerToken)
    {
        httpClient.AddGitHubDefaultHeaders();
        httpClient.AddGitHubBearerToken(bearerToken);

        var response = await httpClient.GetAsync(GITHUB_USER_URL);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("[GitHub][OAuth] Failed to retrieve user information!");
        }

        var content = await response.Content.ReadFromJsonAsync<GitHubUserInfoDto>();
        
        if(content == null)
        {
            throw new InvalidOperationException("[GitHub][OAuth] Invalid response body!");
        }
        
        return content;
    }

    public static async Task<string> GetBearerTokenAsync(this HttpClient httpClient, string authorizationCode, string clientId, string clientSecret)
    {
        httpClient.AddGitHubDefaultHeaders();

        var requestBody = new
        {
            client_id = clientId,
            client_secret = clientSecret,
            code = authorizationCode,
            grant_type = "authorization_code"
        };

        var response = await httpClient.PostAsJsonAsync(GITHUB_ACCESS_TOKEN_URL, requestBody);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to retrieve access token!");
        }

        var content = await response.Content.ReadFromJsonAsync<GitHubTokenResponseDto>();

        if(content == null)
        {
            throw new InvalidOperationException("[GitHub][OAuth] Invalid response body!");
        }

        return content.AccessToken;
    }
}
