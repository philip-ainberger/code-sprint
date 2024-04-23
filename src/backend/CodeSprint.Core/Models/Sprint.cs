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
    Guid[] Tags) : BaseUserRefModel(Id, UserId);