namespace CodeSprint.Core.Models;

public record RefreshToken(
    Guid Id,
    Guid UserId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string Token
) : BaseUserRefModel(Id, UserId);