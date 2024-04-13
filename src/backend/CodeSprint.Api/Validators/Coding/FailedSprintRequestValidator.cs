using CodeSprint.Common.Grpc.Coding;
using FluentValidation;

namespace CodeSprint.Api.Validators.Coding;

public class FailedSprintRequestValidator : BaseRequestValidator<FailedSprintRequest>
{
    public FailedSprintRequestValidator()
    {
        RuleFor(c => c.Id).NotEmpty().Must(BeValidGuid);
    }
}