using CodeSprint.Api.Mapping;
using CodeSprint.Api.Services;
using CodeSprint.Common.Grpc.Tagging;
using CodeSprint.Core.Repositories;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace CodeSprint.Api.Grpc;

[Authorize]
public class TaggingService : TaggingGrpcService.TaggingGrpcServiceBase
{
    private readonly ITaggingRepository _taggingRepository;
    private readonly Guid _userId;

    public TaggingService(ITaggingRepository taggingRepository, ISessionProviderService sessionProviderService)
    {
        _taggingRepository = taggingRepository;
        _userId = sessionProviderService.GetCurrentSessionUserId();
    }

    public override async Task<Tag> CreateTag(CreateTagRequest request, ServerCallContext context)
    {
        var entity = await _taggingRepository.AddAsync(_userId, request.ToEntity(_userId));

        return entity.ToProto();
    }

    public override async Task<Empty> DeleteTag(DeleteTagRequest request, ServerCallContext context)
    {
        // TODO check if id exists (maybe new any function on repository)

        var deleted = await _taggingRepository.RemoveAsync(_userId, Guid.Parse(request.Id));

        // validate deleted, do same on other services

        return new Empty();
    }

    public override async Task<Tag> GetTag(GetTagRequest request, ServerCallContext context)
    {
        var entity = await _taggingRepository.GetByIdAsync(_userId, Guid.Parse(request.Id));

        if (entity == null)
        {
            // TODO log
            throw new RpcException(new Status(StatusCode.NotFound, string.Format("Could not find tag '{0}'", request.Id)));
        }

        return entity.ToProto();
    }

    public override async Task<ListTagsResponse> ListTags(ListTagsRequest request, ServerCallContext context)
    {
        var entites = await _taggingRepository.GetAllAsync(_userId);

        return new ListTagsResponse() { Tags = { entites.Select(TaggingMapExtensions.ToProto) } };
    }

    public override async Task<Tag> UpdateTag(UpdateTagRequest request, ServerCallContext context)
    {
        var entity = await _taggingRepository.GetByIdAsync(_userId, Guid.Parse(request.Id));

        if (entity == null)
        {
            // TODO log
            throw new RpcException(new Status(StatusCode.NotFound, string.Format("Could not find tag '{0}'", request.Id)));
        }

        entity = entity with
        {
            Name = request.Name
        };

        var updatedEntity = await _taggingRepository.UpdateAsync(_userId, entity);

        return updatedEntity.ToProto();
    }
}