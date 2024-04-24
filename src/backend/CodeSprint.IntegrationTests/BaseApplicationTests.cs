using CodeSprint.Core.Enums;
using CodeSprint.Core.Models;
using CodeSprint.Tests.Common;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace CodeSprint.IntegrationTests;

[Collection("ServiceTests")]
public class BaseApplicationTests : IClassFixture<ApiWebApplicationFactory<Program>>, IDisposable
{
    internal ApiWebApplicationFactory<Program> _factory;
    internal readonly MongoConnectorProvider _mongoCollectionProvider;

    private protected BaseApplicationTests(ApiWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _mongoCollectionProvider = new MongoConnectorProvider(_factory.MongoConnectionString);
    }

    public GrpcChannel GetChannel()
    {
        var httpClient = _factory.CreateDefaultClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));

        if (httpClient.BaseAddress == null)
        {
            throw new ArgumentException("Base address is not set");
        }

        var token = _factory.GetJwtToken();
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        return GrpcChannel.ForAddress(httpClient.BaseAddress, new GrpcChannelOptions { HttpClient = httpClient });
    }

    public async Task ArrangeTagsAsync(params Tag[] tags)
    {
        var collection = _mongoCollectionProvider.GetCollection<Tag>("tags");
        await collection.InsertManyAsync(tags);
    }

    public async Task ArrangeSprintsAsync(params Sprint[] sprints)
    {
        var collection = _mongoCollectionProvider.GetCollection<Sprint>("sprints");
        await collection.InsertManyAsync(sprints);
    }

    public static Tag CreateTestTag() => new(Guid.NewGuid(), Guid.Empty, "Tag");

    public static Sprint CreateTestSprint(Languages language = Languages.CSharp, params Guid[] tags) => new(
            Guid.NewGuid(),
            Guid.Empty,
            DateTime.UtcNow,
            "Test",
            "Test",
            "Test",
            "Test",
            0,
            0,
            language,
            tags);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _mongoCollectionProvider.MongoClient.DropDatabase("default");
        }
    }
}