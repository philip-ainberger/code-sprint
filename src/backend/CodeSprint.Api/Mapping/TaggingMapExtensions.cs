using Google.Protobuf.Collections;

namespace CodeSprint.Api.Mapping;

public static class TaggingMapExtensions
{
    public static Common.Grpc.Tagging.Tag ToProto(this Core.Models.Tag model)
    {
        return new Common.Grpc.Tagging.Tag
        {
            Id = model.Id.ToString(),
            UserId = model.UserId.ToString(),
            Name = model.Name
        };
    }

    public static Core.Models.Tag ToEntity(this Common.Grpc.Tagging.CreateTagRequest proto, Guid userId)
    {
        return new Core.Models.Tag(
            Guid.NewGuid(),
            userId,
            proto.Name
        );
    }


    public static Core.Models.Tag ToEntity(this Common.Grpc.Tagging.Tag proto)
    {
        return new Core.Models.Tag(
            Guid.Parse(proto.Id),
            Guid.Parse(proto.UserId),
            proto.Name
        );
    }

    public static Core.Models.Tag[] ToEntity(this RepeatedField<Common.Grpc.Tagging.Tag> proto)
    {
        return proto.Select(ToEntity).ToArray();
    }

    public static Guid[] ToEntityIds(this RepeatedField<string> proto)
    {
        return proto.Select(Guid.Parse).ToArray();
    }
}