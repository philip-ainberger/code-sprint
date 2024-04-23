using CodeSprint.Common.Grpc.Coding;
using FluentValidation;

namespace CodeSprint.Api.Validators.Coding;

public class ListSprintsRequestValidator : BaseRequestValidator<ListSprintsRequest>
{
    public ListSprintsRequestValidator()
    {
        RuleFor(c => c.Filter.Languages)
            .NotEmpty()
            .When(c => c.Filter != null);
        
        RuleFor(c => c.Filter.Tags)
            .NotEmpty()
            .When(c => c.Filter != null);
    }
}