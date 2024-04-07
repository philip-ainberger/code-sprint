namespace CodeSprint.Core.Models;

public record User(
    Guid Id, 
    string ExternalId, 
    string Name, 
    string Email, 
    string ProfilePictureUrl, 
    DateTime CreatedAt) : BaseModel(Id);