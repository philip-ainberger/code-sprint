namespace CodeSprint.Common.Options;

public class GithubOAuthOptions
{
    public const string Section = "GitHub";

    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
}