namespace CodeSprint.Api.Services;

public interface ISessionProviderService
{
    Guid GetCurrentSessionUserId();
}