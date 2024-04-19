using CodeSprint.Common.Grpc.Tagging;
using FluentValidation;

namespace CodeSprint.Api.Validators.Tagging;

public class ListTagsRequestValidator : BaseRequestValidator<ListTagsRequest>
{
    public ListTagsRequestValidator()
    {
        RuleFor(c => c.UserId).NotEmpty().Must(BeValidGuid);
    }
}