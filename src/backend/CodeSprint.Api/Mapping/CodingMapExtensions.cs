using CodeSprint.Api.Repositories.Extensions;
using CodeSprint.Core.Repositories;
using Google.Protobuf.WellKnownTypes;

namespace CodeSprint.Api.Mapping;

public static class CodingMapExtensions
{
    public static Common.Grpc.Coding.Sprint ToProto(this Core.Models.Sprint model, ITaggingRepository taggingRepository)
    {
        return new Common.Grpc.Coding.Sprint
        {
            Id = model.Id.ToString(),
            UserId = model.UserId.ToString(),
            Language = model.Language.ToProtoLanguage(),
            CreatedAt = model.CreatedAt.ToTimestamp(),
            Title = model.Title,
            Description = model.Description,
            SolvedCount = model.SolvedCount,
            FailedCount = model.FailedCount,
            CodeExercise = model.CodeExercise,
            CodeSolution = model.CodeSolution,
            Tags = { taggingRepository.ResolveTagsById(model.Tags, model.UserId).Select(TaggingMapExtensions.ToProto) },
            History = { model.History.Select(ToProto) }
        };
    }

    public static Core.Models.Sprint ToEntity(this Common.Grpc.Coding.Sprint proto)
    {
        return new Core.Models.Sprint(
            Guid.Parse(proto.Id),
            Guid.Parse(proto.UserId),
            proto.CreatedAt.ToDateTime(),
            proto.Title,
            proto.Description,
            proto.CodeSolution,
            proto.CodeExercise,
            proto.SolvedCount,
            proto.FailedCount,
            proto.Language.ToEntityLanguage(),
            proto.Tags.Select(c => Guid.Parse(c.Id)).ToArray(),
            proto.History.Select(ToEntity).ToArray()
        );
    }
    
    public static Core.Models.Sprint ToEntity(this Common.Grpc.Coding.CreateSprintRequest proto, Guid userId, ITaggingRepository taggingRepository)
    {
        return new Core.Models.Sprint(
            Guid.NewGuid(),
            userId,
            DateTime.UtcNow,
            proto.Title,
            proto.Description,
            proto.CodeSolution,
            proto.CodeExercise,
            default,
            default,
            proto.Language.ToEntityLanguage(),
            taggingRepository.ResolveTagsById(proto.Tags.ToArray(), userId).Select(c => c.Id).ToArray(),
            Array.Empty<Core.Models.SprintHistory>()
        );
    }

    internal static Common.Grpc.Coding.Language ToProtoLanguage(this Core.Enums.Languages language)
    {
        return language switch
        {
            Core.Enums.Languages.CSharp => Common.Grpc.Coding.Language.Csharp,
            Core.Enums.Languages.Powershell => Common.Grpc.Coding.Language.Powershell,
            _ => Common.Grpc.Coding.Language.None,
        };
    }

    internal static Core.Enums.Languages ToEntityLanguage(this Common.Grpc.Coding.Language language)
    {
        return language switch
        {
            Common.Grpc.Coding.Language.Csharp => Core.Enums.Languages.CSharp,
            Common.Grpc.Coding.Language.Powershell => Core.Enums.Languages.Powershell,
            _ => Core.Enums.Languages.Unknown,
        };
    }

    public static Common.Grpc.Coding.SprintHistory ToProto(this Core.Models.SprintHistory model)
    {
        return new Common.Grpc.Coding.SprintHistory
        {
            Timestamp = Timestamp.FromDateTime(model.Timestamp.ToUniversalTime()),
            Solved = model.Solved
        };
    }

    public static Core.Models.SprintHistory ToEntity(this Common.Grpc.Coding.SprintHistory proto)
    {
        return new Core.Models.SprintHistory(
            proto.Timestamp.ToDateTime(),
            proto.Solved
        );
    }
}
