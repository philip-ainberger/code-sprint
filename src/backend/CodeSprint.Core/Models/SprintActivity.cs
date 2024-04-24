namespace CodeSprint.Core.Models;

public record SprintActivity(
    Guid Id,
    Guid UserId,
    Guid SprintId,
    DateTime Timestamp,
    bool Solved) : BaseUserRefModel(Id, UserId);