using CodeSprint.Common.Grpc.Coding;
using FluentValidation;

namespace CodeSprint.Api.Validators.Coding;

public class ListSprintsRequestValidator : BaseRequestValidator<ListSprintsRequest>
{
    public ListSprintsRequestValidator()
    {
        RuleFor(c => c.Filter.Languages)
            .NotNull()
            .When(c => c.Filter != null);

        RuleFor(c => c.Filter.Tags)
            .NotNull()
            .When(c => c.Filter != null);
    }
}