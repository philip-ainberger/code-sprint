using CodeSprint.Core.Repositories;
using MongoDB.Driver;

namespace CodeSprint.Api.Repositories;

public class TaggingRepository : UserRefRepository<Core.Models.Tag>, ITaggingRepository
{
    public TaggingRepository(IMongoCollection<Core.Models.Tag> collection) : base(collection)
    {
    }
}