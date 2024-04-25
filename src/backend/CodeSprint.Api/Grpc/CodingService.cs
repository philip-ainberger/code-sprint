using CodeSprint.Api.Mapping;
using CodeSprint.Api.Services;
using CodeSprint.Common.Exceptions;
using CodeSprint.Common.Grpc.Coding;
using CodeSprint.Core.Models;
using CodeSprint.Core.Repositories;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace CodeSprint.Api.Grpc;

[Authorize]
public class CodingService(
    ICodingRepository repository,
    ITaggingRepository taggingRepository,
    ISessionProviderService sessionProviderService,
    ISprintActivityRepository sprintActivityRepository) : CodingGrpcService.CodingGrpcServiceBase
{
    private readonly ICodingRepository _repository = repository;
    private readonly ISprintActivityRepository _sprintActivityRepository = sprintActivityRepository;
    private readonly ITaggingRepository _taggingRepository = taggingRepository;
    private readonly Guid _userId = sessionProviderService.GetCurrentSessionUserId();

    public override async Task<CreateSprintResponse> CreateSprint(CreateSprintRequest request, ServerCallContext context)
    {
        var result = await _repository.AddAsync(_userId, request.ToEntity(_userId, _taggingRepository));

        return new CreateSprintResponse() { Sprint = result.ToProto(_taggingRepository) };
    }

    public override async Task<Empty> DeleteSprint(DeleteSprintRequest request, ServerCallContext context)
    {
        await _repository.RemoveAsync(_userId, Guid.Parse(request.Id));

        return new Empty();
    }

    public override async Task<Empty> FailedSprint(FailedSprintRequest request, ServerCallContext context)
    {
        await CompletedSprintAsync(request.Id, false);

        return new Empty();
    }

    public override async Task<Common.Grpc.Coding.Sprint> GetSprint(GetSprintRequest request, ServerCallContext context)
    {
        var entity = await _repository.GetByIdAsync(_userId, Guid.Parse(request.Id));

        return entity == null
            ? throw new EntityNotFoundException(string.Format("Could not find sprint '{0}'", request.Id))
            : entity.ToProto(_taggingRepository);
    }

    public override async Task<ListSprintsResponse> ListSprints(ListSprintsRequest request, ServerCallContext context)
    {
        var entities = await _repository.GetByFilterAsync(
            _userId,
            request.Page,
            request.Filter?.Languages?.ToEntityLanguages() ?? [],
            request.Filter?.Tags?.ToEntityIds() ?? []);

        var totalCount = await _repository.CountByFilterAsync(_userId,
            request.Filter?.Languages?.ToEntityLanguages() ?? [],
            request.Filter?.Tags?.ToEntityIds() ?? []);

        return new ListSprintsResponse()
        {
            Sprints = { entities.Select(c => c.ToProto(_taggingRepository)) },
            TotalCount = totalCount
        };
    }

    public override async Task<Empty> SolvedSprint(SolvedSprintRequest request, ServerCallContext context)
    {
        await CompletedSprintAsync(request.Id, true);

        return new Empty();
    }

    public override async Task<UpdateSprintResponse> UpdateSprint(UpdateSprintRequest request, ServerCallContext context)
    {
        var entity = await _repository.GetByIdAsync(_userId, Guid.Parse(request.Id));

        if (entity == null)
            throw new EntityNotFoundException(string.Format("Could not find sprint '{0}'", request.Id));

        entity = entity with
        {
            Title = request.Title,
            Description = request.Description,
            CodeExercise = request.CodeExercise,
            CodeSolution = request.CodeSolution,
            Language = request.Language.ToEntityLanguage(),
            Tags = request.Tags.ToEntityIds()
        };

        var updatedEntity = await _repository.UpdateAsync(_userId, entity);

        return new UpdateSprintResponse()
        {
            Sprint = updatedEntity.ToProto(_taggingRepository)
        };
    }

    public override Task<GetCodingActivityResponse> GetCodingActivity(GetCodingActivityRequest request, ServerCallContext context)
    {
        var activities = _sprintActivityRepository.GetActivities(_userId);

        return Task.FromResult(activities.ToProtoResponse());
    }

    private async Task CompletedSprintAsync(string id, bool successfull)
    {
        var entity = await _repository.GetByIdAsync(_userId, Guid.Parse(id));

        if (entity == null)
            throw new EntityNotFoundException(string.Format("Could not find sprint '{0}'", id));

        entity = successfull
            ? entity with { SolvedCount = entity.SolvedCount + 1 }
            : entity with { FailedCount = entity.FailedCount + 1 };

        await _repository.UpdateAsync(_userId, entity);

        await _sprintActivityRepository.AddAsync(_userId, new SprintActivity(Guid.NewGuid(), _userId, Guid.Parse(id), DateTime.UtcNow, successfull));
    }
}