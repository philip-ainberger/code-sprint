using CodeSprint.Core.Models;
using CodeSprint.Core.Repositories;
using MongoDB.Driver;

namespace CodeSprint.Api.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IMongoCollection<RefreshToken> _collection;

    public RefreshTokenRepository(IMongoCollection<RefreshToken> collection)
    {
        _collection = collection;
    }

    public async Task<RefreshToken> AddOrOverrideAsync(Guid userId, string token)
    {
        var filter = Builders<RefreshToken>.Filter.Eq(s => s.UserId, userId);
        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        document = document != null
            ? document with { UserId = userId, UpdatedAt = DateTime.UtcNow, Token = token }
            : new RefreshToken(Guid.NewGuid(), userId, DateTime.UtcNow, DateTime.UtcNow, token);

        var options = new FindOneAndReplaceOptions<RefreshToken, RefreshToken>
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };

        return await _collection.FindOneAndReplaceAsync(filter, document, options);
    }

    public async Task<RefreshToken> GetByUserIdAsync(Guid userId)
    {
        var filter = Builders<RefreshToken>.Filter.Eq(s => s.UserId, userId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}