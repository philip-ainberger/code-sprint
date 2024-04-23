using CodeSprint.Core.Models;
using CodeSprint.Core.Repositories;
using MongoDB.Driver;

namespace CodeSprint.Api.Repositories;

public class SprintActivityRepository : UserRefRepository<SprintActivity>, ISprintActivityRepository
{
    public SprintActivityRepository(IMongoCollection<SprintActivity> collection) : base(collection)
    {
    }
}