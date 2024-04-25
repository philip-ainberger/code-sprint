using System.ComponentModel.DataAnnotations;

namespace CodeSprint.Common.Options;

public class JwtOptions
{
    public const string AppSettingsSection = "Jwt";

    [Required]
    public required string AccessTokensKey { get; set; }
    [Required]
    public required string RefreshTokensKey { get; set; }
    [Required]
    public string ValidIssuer { get; set; } = "code-sprint";
    [Required]
    public string ValidAudience { get; set; } = "code-sprint-api";
}
