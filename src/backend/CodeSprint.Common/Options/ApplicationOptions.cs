using System.ComponentModel.DataAnnotations;

namespace CodeSprint.Common.Options;

public class ApplicationOptions
{
    public const string AppSettingsSection = "Application";

    [Required]
    public required string HostedClientUrl { get; set; }

    public bool HttpOnly { get; set; } = false;
}