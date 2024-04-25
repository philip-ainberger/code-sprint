using System.ComponentModel.DataAnnotations;

namespace CodeSprint.Common.Options;

public class GitHubOAuthOptions
{
    public const string AppSettingsSection = "GitHub";

    [Required]
    public required string ClientId { get; set; }
    [Required]
    public required string ClientSecret { get; set; }
    [Required]
    public required string OAuthAccessTokenEndpoint { get; set; }
    [Required]
    public required string OAuthClientAuthorizationEndpoint { get; set; }
    [Required]
    public required string UserApiEndpoint { get; set; }
}