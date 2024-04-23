using CodeSprint.Core.Repositories;
using FluentAssertions;
using Mongo2Go;
using MongoDB.Driver;

namespace CodeSprint.Tests.Tests;

public class UserRefRepositoryTests
{
    private const string DatabaseName = "default";
    private const string CollectionName = "temp-collection";
    private readonly IUserRefRepository<TestModel> _repository;
    private readonly MongoClient _mongoClient;
    private readonly MongoDbRunner _mongoRunner;

    public UserRefRepositoryTests()
    {
        _mongoRunner = MongoDbRunner.Start();
        _mongoClient = new MongoClient(_mongoRunner.ConnectionString);

        var collection = _mongoClient
            .GetDatabase(DatabaseName)
        .GetCollection<TestModel>(CollectionName);

        _repository = new TestRepository(collection);
    }

    private TestModel CreateTestModel()
    {
        return new TestModel(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test"
        );
    }

    [Fact]
    public async Task AddAsync_AddsModelSuccessfully()
    {
        // arrange
        var model = CreateTestModel();

        // act
        var result = await _repository.AddAsync(model.UserId, model);

        // assert
        result.Should().NotBeNull();
        result.Id.Should().Be(model.Id);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsModelsForUser()
    {
        // arrange
        var userId = Guid.NewGuid();
        var model = CreateTestModel() with { UserId = userId };

        await _repository.AddAsync(userId, model);
        await _repository.AddAsync(Guid.NewGuid(), CreateTestModel());

        // act
        var result = await _repository.GetAllAsync(userId);

        // assert
        result.Should().NotBeNull();
        result.Should().ContainSingle();
        result.Should().Contain(c => c.Id == model.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectModel()
    {
        // arrange
        var model = CreateTestModel();
        await _repository.AddAsync(model.UserId, model);
        await _repository.AddAsync(Guid.NewGuid(), CreateTestModel());

        // act
        var result = await _repository.GetByIdAsync(model.UserId, model.Id);

        // assert
        result.Should().NotBeNull();
        result.Id.Should().Be(model.Id);
    }

    [Fact]
    public async Task RemoveAsync_RemovesModelSuccessfully()
    {
        // arrange
        var model = CreateTestModel();
        await _repository.AddAsync(model.UserId, model);

        // act
        await _repository.RemoveAsync(model.UserId, model.Id);

        var result = await _repository.GetByIdAsync(model.UserId, model.Id);

        // assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesModelSuccessfully()
    {
        // arrange
        var model = CreateTestModel();
        await _repository.AddAsync(model.UserId, model);

        // act
        var updatedModel = model with { Name = "Updated Name" };
        var result = await _repository.UpdateAsync(model.UserId, updatedModel);

        // assert
        result.Should().NotBeNull();
        result.Name.Should().Be(updatedModel.Name);
    }

    public void Dispose()
    {
        _mongoRunner.Dispose();
    }
}