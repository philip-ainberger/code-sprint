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
    Language Language,
    Guid[] Tags,
    SprintHistory[] History) : BaseUserRefModel(Id, UserId);

public record SprintHistory(DateTime Timestamp, bool Solved);