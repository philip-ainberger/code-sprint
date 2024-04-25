namespace CodeSprint.Common.Options;

public class ApplicationOptions
{
    public const string Section = "Application";

    public required string HostedClientUrl { get; set; }
}