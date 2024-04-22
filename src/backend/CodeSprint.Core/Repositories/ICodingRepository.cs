using CodeSprint.Core.Enums;
using CodeSprint.Core.Models;

namespace CodeSprint.Core.Repositories;

public interface ICodingRepository : IUserRefDataRepository<Sprint>
{
    Task<uint> CountAllAsync(Guid userId);
    Task<ICollection<Sprint>> GetByFilterAsync(Guid userId, uint page, Languages[] languages, Guid[] tagIds);
    Task<uint> CountByFilterAsync(Guid userId, Languages[] languages, Guid[] tagIds);
    ICollection<CodingActivity> GetCodingActivity(Guid userId);
}