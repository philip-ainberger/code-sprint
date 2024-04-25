using CodeSprint.Api.Repositories.Extensions;
using CodeSprint.Core.Repositories;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using System.Diagnostics.CodeAnalysis;

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
            Tags = { taggingRepository.ResolveTagsById(model.Tags, model.UserId).Select(TaggingMapExtensions.ToProto) }
        };
    }
    
    public static Common.Grpc.Coding.GetCodingActivityResponse ToProtoResponse(this IDictionary<DateTime, int> entity)
    {
        return new Common.Grpc.Coding.GetCodingActivityResponse()
        {
            Activities = { entity.Select(c => new Common.Grpc.Coding.CodingActivity() { Timestamp = c.Key.ToTimestamp(), Count = c.Value }) }
        };
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
            taggingRepository.ResolveTagsById(proto.Tags.ToArray(), userId).Select(c => c.Id).ToArray()
        );
    }

    [ExcludeFromCodeCoverage]
    internal static Common.Grpc.Coding.Language ToProtoLanguage(this Core.Enums.Languages language)
    {
        return language switch
        {
            Core.Enums.Languages.CSharp => Common.Grpc.Coding.Language.Csharp,
            Core.Enums.Languages.Powershell => Common.Grpc.Coding.Language.Powershell,
            Core.Enums.Languages.Typescript => Common.Grpc.Coding.Language.Typescript,
            Core.Enums.Languages.Java => Common.Grpc.Coding.Language.Java,
            Core.Enums.Languages.Javascript => Common.Grpc.Coding.Language.Javascript,
            Core.Enums.Languages.Go => Common.Grpc.Coding.Language.Go,
            Core.Enums.Languages.Markdown => Common.Grpc.Coding.Language.Markdown,
            Core.Enums.Languages.Dockerfile => Common.Grpc.Coding.Language.Dockerfile,
            Core.Enums.Languages.Sql => Common.Grpc.Coding.Language.Sql,
            _ => Common.Grpc.Coding.Language.None,
        };
    }

    [ExcludeFromCodeCoverage]
    internal static Core.Enums.Languages ToEntityLanguage(this Common.Grpc.Coding.Language language)
    {
        return language switch
        {
            Common.Grpc.Coding.Language.Csharp => Core.Enums.Languages.CSharp,
            Common.Grpc.Coding.Language.Powershell => Core.Enums.Languages.Powershell,
            Common.Grpc.Coding.Language.Typescript => Core.Enums.Languages.Typescript,
            Common.Grpc.Coding.Language.Javascript => Core.Enums.Languages.Javascript,
            Common.Grpc.Coding.Language.Java => Core.Enums.Languages.Java,
            Common.Grpc.Coding.Language.Sql => Core.Enums.Languages.Sql,
            Common.Grpc.Coding.Language.Markdown => Core.Enums.Languages.Markdown,
            Common.Grpc.Coding.Language.Dockerfile => Core.Enums.Languages.Dockerfile,
            Common.Grpc.Coding.Language.Go => Core.Enums.Languages.Go,
            _ => Core.Enums.Languages.Unknown,
        };
    }

    internal static Core.Enums.Languages[] ToEntityLanguages(this RepeatedField<Common.Grpc.Coding.Language> languages)
    {
        return languages?.Select(ToEntityLanguage).ToArray() ?? [];
    }
}
