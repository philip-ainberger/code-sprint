namespace CodeSprint.Core.Models;

public abstract record BaseUserRefModel(Guid Id, Guid UserId) : BaseModel(Id);