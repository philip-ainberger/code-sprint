using CodeSprint.Tests.Common;

namespace CodeSprint.IntegrationTests;

[Collection("ServiceTests")]
public class BaseApplicationTest : IClassFixture<ApiWebApplicationFactory<Program>>, IDisposable
{
    internal ApiWebApplicationFactory<Program> _factory;
    internal readonly MongoConnectorProvider _mongoCollectionProvider;

    private protected BaseApplicationTest(ApiWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _mongoCollectionProvider = new MongoConnectorProvider(_factory.MongoConnectionString);
    }

    public void Dispose()
    {
        _mongoCollectionProvider.MongoClient.DropDatabase("default");
    }
}