using System.Text.Json.Serialization;

namespace CodeSprint.Common.Dtos;

public class GitHubUserInfoDto
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("node_id")]
    public required string NodeId { get; set; }

    [JsonPropertyName("login")]
    public required string Login { get; set; }

    [JsonPropertyName("avatar_url")]
    public required string AvatarUrl { get; set; }

    [JsonPropertyName("email")]
    public required string Email { get; set; }
}