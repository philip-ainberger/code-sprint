using CodeSprint.Common.Grpc.Coding;
using FluentValidation;

namespace CodeSprint.Api.Validators.Coding;

public class CreateSprintRequestValidator : BaseRequestValidator<CreateSprintRequest>
{
    public CreateSprintRequestValidator()
    {
        RuleFor(c => c.Title).NotNull().Length(1, 100);
        RuleFor(c => c.Description).MaximumLength(1000);
        RuleFor(c => c.CodeExercise).NotNull().NotEmpty();
        RuleFor(c => c.CodeSolution).NotNull().NotEmpty();
        RuleFor(c => c.Language).NotNull().NotEmpty();
        RuleForEach(c => c.Tags)
            .Must(BeValidGuid)
            .When(c => c.Tags?.Count > 0);
    }
}