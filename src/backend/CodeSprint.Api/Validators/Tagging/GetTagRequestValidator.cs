using CodeSprint.Common.Grpc.Tagging;
using FluentValidation;

namespace CodeSprint.Api.Validators.Tagging;

public class GetTagRequestValidator : BaseRequestValidator<GetTagRequest>
{
    public GetTagRequestValidator()
    {
        RuleFor(c => c.Id).NotEmpty().Must(BeValidGuid);
    }
}