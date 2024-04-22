namespace CodeSprint.Common.Options;

public class JwtOptions
{
    public const string Section = "Jwt";

    public required string Key { get; set; }
    public string ValidIssuer => "code-sprint";
    public string ValidAudience => "code-sprint-api";
}
