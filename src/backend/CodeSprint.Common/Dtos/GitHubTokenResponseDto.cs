using System.Text.Json.Serialization;

namespace CodeSprint.Common.Dtos;

public class GitHubTokenResponseDto
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }
}