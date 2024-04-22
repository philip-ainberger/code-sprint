using CodeSprint.Core.Enums;

namespace CodeSprint.Core.Models;

public record Sprint(
    Guid Id,
    Guid UserId,
    DateTime CreatedAt,
    string Title,
    string Description,
    string CodeSolution,
    string CodeExercise,
    uint SolvedCount,
    uint FailedCount,
    Languages Language,
    Guid[] Tags,
    SprintHistory[] History,
    DateTime? DeletedAt = null) : BaseUserRefModel(Id, UserId, DeletedAt);

public record SprintHistory(DateTime Timestamp, bool Solved);