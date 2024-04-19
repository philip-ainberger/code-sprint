using CodeSprint.Common.Grpc.Coding;
using FluentValidation;

namespace CodeSprint.Api.Validators.Coding;

public class ListSprintsRequestValidator : BaseRequestValidator<ListSprintsRequest>
{
    public ListSprintsRequestValidator()
    {
        RuleFor(c => c.UserId).NotEmpty().Must(BeValidGuid);
    }
}