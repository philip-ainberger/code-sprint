using CodeSprint.Common.Grpc.Tagging;
using FluentValidation;

namespace CodeSprint.Api.Validators.Tagging;

public class UpdateTagRequestValidator : BaseRequestValidator<UpdateTagRequest>
{
    public UpdateTagRequestValidator()
    {
        RuleFor(c => c.Id).NotEmpty().Must(BeValidGuid);
    }
}