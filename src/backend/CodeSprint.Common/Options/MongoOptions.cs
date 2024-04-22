namespace CodeSprint.Common.Options;

public class MongoOptions
{
    public const string Section = "MongoDb";

    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
}