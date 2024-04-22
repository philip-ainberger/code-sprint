using CodeSprint.Core.Models;

namespace CodeSprint.Core.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> AddOrOverrideAsync(Guid userId, string token);
    Task<RefreshToken> GetByUserIdAsync(Guid userId);
}