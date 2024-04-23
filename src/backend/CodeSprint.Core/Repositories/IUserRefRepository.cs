using CodeSprint.Core.Models;

namespace CodeSprint.Core.Repositories;

public interface IUserRefRepository<T> where T : BaseModel
{
    Task<T> AddAsync(Guid userId, T entity);
    Task<T> UpdateAsync(Guid userId, T entity);
    Task<bool> RemoveAsync(Guid userId, Guid id);
    Task<T> GetByIdAsync(Guid userId, Guid id);
    Task<ICollection<T>> GetAllAsync(Guid userId);
    T GetById(Guid userId, Guid id);
}
