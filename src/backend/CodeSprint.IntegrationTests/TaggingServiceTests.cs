using CodeSprint.Common.Grpc.Tagging;
using FluentAssertions;
using MongoDB.Driver;

namespace CodeSprint.IntegrationTests;

public class TaggingServiceTests(ApiWebApplicationFactory<Program> factory) : BaseApplicationTests(factory)
{
    [Fact]
    public async Task CreateTagRequest_DefaultTag_Created()
    {
        // Arrange 
        var client = new TaggingGrpcService.TaggingGrpcServiceClient(GetChannel());

        // Act
        var request = new CreateTagRequest()
        {
            Name = "Test"
        };
        var response = await client.CreateTagAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().NotBeNull().And.NotBeEmpty();
        response.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task DeleteTagRequest_MultipleTags_CorrectTagDeleted()
    {
        // Arrange 
        var tagToBeDeleted = CreateTestTag();
        await ArrangeTagsAsync(tagToBeDeleted, CreateTestTag());

        var client = new TaggingGrpcService.TaggingGrpcServiceClient(GetChannel());

        // Act
        var response = await client.DeleteTagAsync(new DeleteTagRequest() { Id = tagToBeDeleted.Id.ToString() });

        // Assert
        response.Should().NotBeNull();

        var collection = _mongoCollectionProvider.GetCollection<Core.Models.Tag>("tags");
        collection.AsQueryable().Count().Should().Be(1);
        collection.AsQueryable().Any(c => c.Id == tagToBeDeleted.Id).Should().BeFalse();
    }

    [Fact]
    public async Task UpdateTagRequest_DefaultTag_TagUpdated()
    {
        // Arrange 
        var tag = CreateTestTag();
        await ArrangeTagsAsync(tag);

        var client = new TaggingGrpcService.TaggingGrpcServiceClient(GetChannel());

        // Act
        var request = new UpdateTagRequest()
        {
            Id = tag.Id.ToString(),
            Name = tag + " UPDATE"
        };
        var response = await client.UpdateTagAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(tag.Id.ToString());
        response.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task GetTagRequest_MultipleTags_ReturnCorrectTag()
    {
        // Arrange 
        var tag = CreateTestTag();
        await ArrangeTagsAsync(tag, CreateTestTag());

        var client = new TaggingGrpcService.TaggingGrpcServiceClient(GetChannel());

        // Act
        var response = await client.GetTagAsync(new GetTagRequest() { Id = tag.Id.ToString() });

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(tag.Id.ToString());
        response.Name.Should().Be(tag.Name);
        response.UserId.Should().Be(tag.UserId.ToString());
    }

    [Fact]
    public async Task ListTagsRequest_MultipleTags_ReturnsAllTags()
    {
        // Arrange 
        await ArrangeTagsAsync(CreateTestTag(), CreateTestTag());

        var client = new TaggingGrpcService.TaggingGrpcServiceClient(GetChannel());

        // Act
        var response = await client.ListTagsAsync(new ListTagsRequest());

        // Assert
        response.Should().NotBeNull();
        response.Tags.Should().HaveCount(2);
    }
}