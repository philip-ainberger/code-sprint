using CodeSprint.Common.Grpc.Coding;
using FluentValidation;

namespace CodeSprint.Api.Validators.Coding;

public class DeleteSprintRequestValidator : BaseRequestValidator<DeleteSprintRequest>
{
    public DeleteSprintRequestValidator()
    {
        RuleFor(c => c.Id).NotEmpty().Must(BeValidGuid);
    }
}