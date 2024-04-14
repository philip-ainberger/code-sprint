using CodeSprint.Commom.Exceptions;
using CodeSprint.Core.Repositories;

namespace CodeSprint.Api.Repositories.Extensions;

public static class TaggingRepositoryExtensions
{
    public static Core.Models.Tag[] ResolveTagsById(this ITaggingRepository repository, Guid[] ids, Guid userId)
    {
        return ids
            .Select(c => repository.GetById(userId, c)
                ?? throw new EntityNotFoundException(string.Format("Tag '{0}' does not exist and is not valid for usage.", c)))
            .ToArray();
    }

    public static Core.Models.Tag[] ResolveTagsById(this ITaggingRepository repository, string[] ids, Guid userId)
    {
        return repository.ResolveTagsById(ids.Select(c => Guid.Parse(c)).ToArray(), userId);
    }
}