namespace CodeSprint.Core.Models;

public record Tag(Guid Id, Guid UserId, string Name) : BaseUserRefModel(Id, UserId);