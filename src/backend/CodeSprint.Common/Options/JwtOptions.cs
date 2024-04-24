namespace CodeSprint.Common.Options;

public class JwtOptions
{
    public const string Section = "Jwt";

    public required string AccessTokensKey { get; set; }
    public required string RefreshTokensKey { get; set; }
    public string ValidIssuer { get; set; } = "code-sprint";
    public string ValidAudience { get; set; } = "code-sprint-api";
}
