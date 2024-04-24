using CodeSprint.Core.Enums;
using CodeSprint.Core.Repositories;
using MongoDB.Driver;

namespace CodeSprint.Api.Repositories;

public class CodingRepository : UserRefRepository<Core.Models.Sprint>, ICodingRepository
{
    private readonly IMongoCollection<Core.Models.Sprint> _collection;
    private const uint PAGE_SIZE = 10;

    public CodingRepository(IMongoCollection<Core.Models.Sprint> collection) : base(collection)
    {
        _collection = collection;
    }

    public async Task<uint> CountAllAsync(Guid userId)
    {
        var filter = Builders<Core.Models.Sprint>.Filter.Eq(s => s.UserId, userId);
        return (uint)await _collection.CountDocumentsAsync(filter);
    }

    public async Task<ICollection<Core.Models.Sprint>> GetByFilterAsync(Guid userId, uint page, Languages[] languages, Guid[] tagIds)
    {
        var filter = BuildFilter(userId, languages, tagIds);

        int skip = (int)(PAGE_SIZE * page);
        return await _collection.Find(filter)
            .SortByDescending(c => c.CreatedAt)
            .Limit((int)PAGE_SIZE)
            .Skip(skip)
            .ToListAsync();
    }

    public async Task<uint> CountByFilterAsync(Guid userId, Languages[] languages, Guid[] tagIds)
    {
        var filter = BuildFilter(userId, languages, tagIds);

        return (uint)await _collection.CountDocumentsAsync(filter);
    }

    private static FilterDefinition<Core.Models.Sprint> BuildFilter(Guid userId, Languages[] languages, Guid[] tagIds)
    {
        var filter = Builders<Core.Models.Sprint>.Filter.Eq(s => s.UserId, userId);

        if (languages != null && languages.Length > 0)
        {
            filter &= Builders<Core.Models.Sprint>.Filter.In(s => s.Language, languages);
        }

        if (tagIds != null && tagIds.Length > 0)
        {
            filter &= Builders<Core.Models.Sprint>.Filter.AnyIn(s => s.Tags, tagIds);
        }

        return filter;
    }
}