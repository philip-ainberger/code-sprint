using MongoDB.Driver;

namespace CodeSprint.Tests.Common;

public class MongoConnectorProvider
{
    private readonly MongoClient _mongoClient;
    private readonly string _connectionString;

    public MongoConnectorProvider(string connectionString)
    {
        _connectionString = connectionString;
        _mongoClient = new MongoClient(_connectionString);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _mongoClient
            .GetDatabase("default")
            .GetCollection<T>(collectionName);
    }

    public IMongoClient MongoClient => new MongoClient(_connectionString);
}