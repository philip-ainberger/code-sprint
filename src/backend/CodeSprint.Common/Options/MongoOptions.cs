namespace CodeSprint.Common.Options;

public class MongoOptions
{
    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
}