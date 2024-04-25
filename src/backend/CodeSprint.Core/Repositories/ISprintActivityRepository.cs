using CodeSprint.Core.Models;

namespace CodeSprint.Core.Repositories;

public interface ISprintActivityRepository : IUserRefRepository<SprintActivity>
{
    IDictionary<DateTime, int> GetActivities(Guid userId);
}