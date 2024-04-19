using CodeSprint.Common.Grpc.Coding;
using FluentValidation;

namespace CodeSprint.Api.Validators.Coding;

public class SolvedSprintRequestValidator : BaseRequestValidator<SolvedSprintRequest>
{
    public SolvedSprintRequestValidator()
    {
        RuleFor(c => c.Id).NotEmpty().Must(BeValidGuid);
    }
}