using CodeSprint.Common.Dtos;
using CodeSprint.Core.Models;

namespace CodeSprint.Common.Extensions;

public static class DtoMappingExtensions
{
    public static User ToNewEntity(this GitHubUserInfoDto userInfo)
    {
        return new User(Guid.NewGuid(), userInfo.Id.ToString(), userInfo.Name, userInfo.Email, userInfo.AvatarUrl, DateTime.UtcNow);
    }
}