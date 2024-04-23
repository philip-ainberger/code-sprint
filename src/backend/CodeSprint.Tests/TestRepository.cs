using CodeSprint.Api.Repositories;
using CodeSprint.Core.Models;
using MongoDB.Driver;

namespace CodeSprint.Tests;

public record TestModel(Guid Id, Guid UserId, string Name) : BaseUserRefModel(Id, UserId);

public class TestRepository : UserRefRepository<TestModel>
{
    public TestRepository(IMongoCollection<TestModel> collection) : base(collection)
    {
    }
}