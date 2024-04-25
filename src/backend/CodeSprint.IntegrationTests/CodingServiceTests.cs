using CodeSprint.Common.Grpc.Coding;
using CodeSprint.Core.Enums;
using FluentAssertions;
using Grpc.Core;
using MongoDB.Driver;

namespace CodeSprint.IntegrationTests;

public class CodingServiceTests(ApiWebApplicationFactory<Program> factory) : BaseApplicationTests(factory)
{
    [Fact]
    public async Task GetSprint_MultipleSprints_ReturnCorrectSprint()
    {
        // Arrange
        var sprint = CreateTestSprint();
        await ArrangeSprintsAsync(CreateTestSprint(), CreateTestSprint(), sprint);

        var client = new CodingGrpcService.CodingGrpcServiceClient(GetChannel());

        // Act
        var response = await client.GetSprintAsync(new GetSprintRequest() { Id = sprint.Id.ToString() });

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(sprint.Id.ToString());
    }

    [Fact]
    public async Task GetSprint_InvalidSprint_ReturnNotFound()
    {
        // Act
        var client = new CodingGrpcService.CodingGrpcServiceClient(GetChannel());
        var request = new GetSprintRequest() { Id = Guid.NewGuid().ToString() };

        // Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => client.GetSprintAsync(request).ResponseAsync);

        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(StatusCode.NotFound);
    }

    [Fact]
    public async Task GetSprint_FailValidationWithInvalidGuid_ReturnInvalidArgument()
    {
        // Act
        var client = new CodingGrpcService.CodingGrpcServiceClient(GetChannel());
        var request = new GetSprintRequest() { Id = "INVALID" };

        // Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => client.GetSprintAsync(request).ResponseAsync);

        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(StatusCode.InvalidArgument);
    }

    [Fact]
    public async Task GetSprints_MultipleSprints_ReturnAllSprints()
    {
        // Arrange
        await ArrangeSprintsAsync(CreateTestSprint(), CreateTestSprint(), CreateTestSprint());

        var client = new CodingGrpcService.CodingGrpcServiceClient(GetChannel());

        // Act
        var response = await client.ListSprintsAsync(new ListSprintsRequest());

        // Assert
        response.Should().NotBeNull();
        response.Sprints.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetSprints_MultipleSprints_ReturnSprintsByFilter()
    {
        // Arrange
        var defaultLanguage = Languages.CSharp;
        var expectedSprint = CreateTestSprint(Languages.Powershell);
        await ArrangeSprintsAsync(CreateTestSprint(defaultLanguage), CreateTestSprint(defaultLanguage), expectedSprint);

        var client = new CodingGrpcService.CodingGrpcServiceClient(GetChannel());

        // Act
        var response = await client.ListSprintsAsync(new ListSprintsRequest()
        {
            Filter = new Filter()
            {
                Languages = { Language.Powershell },
                Tags = { }
            }
        });

        // Assert
        response.Should().NotBeNull();
        response.Sprints.Should().HaveCount(1);
        response.TotalCount.Should().Be(1);
        response.Sprints[0].Id.Should().Be(expectedSprint.Id.ToString());
    }

    [Fact]
    public async Task GetSprints_EmptyDatabase_ReturnEmptyList()
    {
        // Act
        var client = new CodingGrpcService.CodingGrpcServiceClient(GetChannel());
        var response = await client.ListSprintsAsync(new ListSprintsRequest());

        // Assert
        response.Should().NotBeNull();
        response.Sprints.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateSprintRequest_WithoutTags_SprintAdded()
    {
        // Arrange
        var client = new CodingGrpcService.CodingGrpcServiceClient(GetChannel());

        var request = new CreateSprintRequest()
        {
            Title = "Test",
            Description = "Test",
            Language = Language.Csharp,
            CodeExercise = "Exercise",
            CodeSolution = "Solution"
        };

        // Act
        var response = await client.CreateSprintAsync(request);

        // Assert
        AssertResponse(response.Sprint, request);
        response.Sprint.Tags.Should().BeEmpty();

        var collection = _mongoCollectionProvider.GetCollection<CodeSprint.Core.Models.Sprint>("sprints");
        var mongoEntity = await collection.FindAsync(Builders<CodeSprint.Core.Models.Sprint>.Filter.Eq(c => c.Id, Guid.Parse(response.Sprint.Id)));

        mongoEntity.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateSprintRequest_UseUnknownTag_NotFoundException()
    {
        // Arrange
        var client = new CodingGrpcService.CodingGrpcServiceClient(GetChannel());

        var request = new CreateSprintRequest()
        {
            Title = "Test",
            Description = "Test",
            Language = Language.Csharp,
            CodeExercise = "Exercise",
            CodeSolution = "Solution",
            Tags = { Guid.NewGuid().ToString() }
        };

        // Act
        var exception = await Assert.ThrowsAsync<RpcException>(() => client.CreateSprintAsync(request).ResponseAsync);

        // Assert
        exception.StatusCode.Should().Be(StatusCode.NotFound);

        var collection = _mongoCollectionProvider.GetCollection<CodeSprint.Core.Models.Sprint>("sprints");
        var entityCount = collection.AsQueryable().Count();

        entityCount.Should().Be(0);
    }

    [Fact]
    public async Task CreateSprintRequest_UseSpecificTag_UsingCorrectTag()
    {
        // Arrange
        var tag = CreateTestTag();
        await ArrangeTagsAsync(tag);

        var client = new CodingGrpcService.CodingGrpcServiceClient(GetChannel());

        var request = new CreateSprintRequest()
        {
            Title = "Test",
            Description = "Test",
            Language = Language.Csharp,
            CodeExercise = "Exercise",
            CodeSolution = "Solution",
            Tags = { tag.Id.ToString() }
        };

        var response = await client.CreateSprintAsync(request);

        // Assert
        AssertResponse(response.Sprint, request);
        response.Sprint.Tags.Should().NotBeEmpty();

        AssertTag(response.Sprint.Tags[0], tag);
    }

    [Fact]
    public async Task DeleteSprintRequest_DeleteSpecificSprint_DeleteSprint()
    {
        // Arrange
        var sprintToDelete = CreateTestSprint();
        await ArrangeSprintsAsync(CreateTestSprint(), sprintToDelete);

        var client = new CodingGrpcService.CodingGrpcServiceClient(GetChannel());

        // Act
        var response = await client.DeleteSprintAsync(new DeleteSprintRequest() { Id = sprintToDelete.Id.ToString() });

        // Assert
        response.Should().NotBeNull();

        var collection = _mongoCollectionProvider.GetCollection<CodeSprint.Core.Models.Sprint>("sprints");

        collection.AsQueryable().Count().Should().Be(1);
        collection.AsQueryable().Any(c => c.Id == sprintToDelete.Id).Should().BeFalse();
    }

    [Fact]
    public async Task UpdateSprintRequest_UpdateSprint_ValuesUpdated()
    {
        // Arrange
        var tag = CreateTestTag();
        var newTag = CreateTestTag();
        await ArrangeTagsAsync(tag, newTag);

        var sprint = CreateTestSprint(Core.Enums.Languages.CSharp, tag.Id);
        await ArrangeSprintsAsync(sprint);

        var client = new CodingGrpcService.CodingGrpcServiceClient(GetChannel());

        // Act
        var request = new UpdateSprintRequest()
        {
            Id = sprint.Id.ToString(),
            Title = sprint.Title + " UPDATE",
            Description = sprint.Description + " UPDATE",
            CodeExercise = sprint.CodeExercise + " UPDATE",
            CodeSolution = sprint.CodeSolution + " UPDATE",
            Language = Language.Powershell,
            Tags = { newTag.Id.ToString() }
        };

        var response = await client.UpdateSprintAsync(request);

        // Assert
        AssertResponse(response.Sprint, request);
        response.Sprint.SolvedCount.Should().Be(0);
        response.Sprint.FailedCount.Should().Be(0);

        response.Sprint.Tags.Should().HaveCount(1);
        AssertTag(response.Sprint.Tags[0], newTag);
    }

    [Fact]
    public async Task SolveSprintRequest_IncreaseFirstTime_SolvedIncreased()
    {
        // Arrange
        var sprint = CreateTestSprint();
        await ArrangeSprintsAsync(sprint);

        var client = new CodingGrpcService.CodingGrpcServiceClient(GetChannel());

        // Act
        Func<Task> act = async () => await client.SolvedSprintAsync(new SolvedSprintRequest() { Id = sprint.Id.ToString() });

        // Assert
        await act.Should().NotThrowAsync();
        
        var collection = _mongoCollectionProvider.GetCollection<Core.Models.Sprint>("sprints");
        var entity = collection.AsQueryable().First(c => c.Id == sprint.Id);
        entity.SolvedCount.Should().Be(1);
    }

    [Fact]
    public async Task FailedSprintRequest_IncreaseFirstTime_FailedIncreased()
    {
        // Arrange
        var sprint = CreateTestSprint();
        await ArrangeSprintsAsync(sprint);

        var client = new CodingGrpcService.CodingGrpcServiceClient(GetChannel());

        // Act
        Func<Task> act = async () => await client.FailedSprintAsync(new FailedSprintRequest() { Id = sprint.Id.ToString() });

        // Assert
        await act.Should().NotThrowAsync();

        var collection = _mongoCollectionProvider.GetCollection<Core.Models.Sprint>("sprints");
        var entity = collection.AsQueryable().First(c => c.Id == sprint.Id);
        entity.FailedCount.Should().Be(1);
    }

    private static void AssertTag(Common.Grpc.Tagging.Tag responseTag, Core.Models.Tag tag)
    {
        responseTag.Id.Should().Be(tag.Id.ToString());
        responseTag.UserId.Should().Be(tag.UserId.ToString());
        responseTag.Name.Should().Be(tag.Name.ToString());
    }

    private static void AssertResponse(Sprint response, CreateSprintRequest expected)
    {
        response.Should().NotBeNull();
        response.Title.Should().Be(expected.Title);
        response.Description.Should().Be(expected.Description);
        response.Language.Should().Be(expected.Language);
        response.CodeExercise.Should().Be(expected.CodeExercise);
        response.CodeSolution.Should().Be(expected.CodeSolution);
        response.SolvedCount.Should().Be(0);
        response.FailedCount.Should().Be(0);
        response.CreatedAt.ToDateTime().Date.Should().Be(DateTime.Today);
        response.UserId.Should().NotBeNull();
    }

    private static void AssertResponse(Sprint response, UpdateSprintRequest expected)
    {
        response.Should().NotBeNull();
        response.Title.Should().Be(expected.Title);
        response.Description.Should().Be(expected.Description);
        response.Language.Should().Be(expected.Language);
        response.CodeExercise.Should().Be(expected.CodeExercise);
        response.CodeSolution.Should().Be(expected.CodeSolution);
    }
}