using CodeSprint.Core.Models;

namespace CodeSprint.Core.Repositories;

public interface IUserRepository
{
    Task AddAsync(User entity);
    Task<User> GetByIdAsync(Guid userId);
    Task<User> GetByExternalIdAsync(string id);
    Task<User> GetByEmailAsync(string eMail);
}