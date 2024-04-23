using CodeSprint.Core.Models;
using CodeSprint.Core.Repositories;
using MongoDB.Driver;

namespace CodeSprint.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(IMongoCollection<User> collection)
    {
        _collection = collection;
    }

    public async Task AddAsync(User entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public Task<User> GetByEmailAsync(string eMail)
    {
        var filter = Builders<User>.Filter.Eq(s => s.Email, eMail);
        return _collection.Find(filter).FirstOrDefaultAsync();
    }

    public Task<User> GetByExternalIdAsync(string id)
    {
        var filter = Builders<User>.Filter.Eq(s => s.ExternalId, id);
        return _collection.Find(filter).FirstOrDefaultAsync();
    }

    public Task<User> GetByIdAsync(Guid userId)
    {
        var filter = Builders<User>.Filter.Eq(s => s.Id, userId);
        return _collection.Find(filter).FirstOrDefaultAsync();
    }
}