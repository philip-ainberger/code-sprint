using CodeSprint.Core.Models;
using CodeSprint.Core.Repositories;
using MongoDB.Driver;

namespace CodeSprint.Api.Repositories;

public abstract class UserRefRepository<T> : IUserRefRepository<T> where T : BaseUserRefModel
{
    private readonly IMongoCollection<T> _collection;

    protected UserRefRepository(IMongoCollection<T> collection)
    {
        _collection = collection;
    }

    public async Task<T> AddAsync(Guid userId, T entity)
    {
        var document = entity with { UserId = userId };

        await _collection.InsertOneAsync(document);

        return document;
    }

    public async Task<ICollection<T>> GetAllAsync(Guid userId)
    {
        var filter = Builders<T>.Filter.Eq(s => s.UserId, userId);
        var documents = await _collection.Find(filter).ToListAsync();
        return documents;
    }

    public T GetById(Guid userId, Guid id)
    {
        var filter = Builders<T>.Filter.Eq(s => s.Id, id) & Builders<T>.Filter.Eq(s => s.UserId, userId);
        return _collection.Find(filter).FirstOrDefault();
    }

    public async Task<T> GetByIdAsync(Guid userId, Guid id)
    {
        var filter = Builders<T>.Filter.Eq(s => s.Id, id) & Builders<T>.Filter.Eq(s => s.UserId, userId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> RemoveAsync(Guid userId, Guid id)
    {
        var filter = Builders<T>.Filter.Eq(s => s.Id, id) & Builders<T>.Filter.Eq(s => s.UserId, userId);
        var result = await _collection.DeleteOneAsync(filter);

        return result.IsAcknowledged && result.DeletedCount == 1;
    }

    public async Task<T> UpdateAsync(Guid userId, T entity)
    {
        var filter = Builders<T>.Filter.Eq(s => s.Id, entity.Id) & Builders<T>.Filter.Eq(s => s.UserId, userId);
        var options = new FindOneAndReplaceOptions<T, T>
        {
            ReturnDocument = ReturnDocument.After
        };

        var document = await _collection.FindOneAndReplaceAsync(filter, entity with { UserId = userId }, options);
        return document;
    }
}