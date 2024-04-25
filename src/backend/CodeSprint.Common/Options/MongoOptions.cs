using System.ComponentModel.DataAnnotations;

namespace CodeSprint.Common.Options;

public class MongoOptions
{
    public const string AppSettingsSection = "MongoDb";


    [Required]
    public required string ConnectionString { get; set; }

    [Required]
    public string DatabaseName { get; set; }
}