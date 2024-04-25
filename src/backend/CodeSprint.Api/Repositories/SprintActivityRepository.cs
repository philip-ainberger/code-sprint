using CodeSprint.Core.Models;
using CodeSprint.Core.Repositories;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace CodeSprint.Api.Repositories;

public class SprintActivityRepository : UserRefRepository<SprintActivity>, ISprintActivityRepository
{
    public SprintActivityRepository(IMongoCollection<SprintActivity> collection) : base(collection)
    {
    }

    [ExcludeFromCodeCoverage(Justification = "Not coverable via mongo-db binary aka mongo2go")]
    public IDictionary<DateTime, int> GetActivities(Guid userId)
    {
        return _collection
            .AsQueryable()
            .Where(c => c.UserId == userId)
            .GroupBy(c => c.Timestamp.Date)
            .ToDictionary(k => k.Key, v => v.Count());
    }
}