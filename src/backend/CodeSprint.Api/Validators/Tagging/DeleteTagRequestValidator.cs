using CodeSprint.Common.Grpc.Tagging;
using FluentValidation;

namespace CodeSprint.Api.Validators.Tagging;

public class DeleteTagRequestValidator : BaseRequestValidator<DeleteTagRequest>
{
    public DeleteTagRequestValidator()
    {
        RuleFor(c => c.Id).NotEmpty().Must(BeValidGuid);
    }
}