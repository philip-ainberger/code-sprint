using CodeSprint.Common.Grpc.Coding;
using FluentValidation;

namespace CodeSprint.Api.Validators.Coding;

public class GetSprintRequestValidator : BaseRequestValidator<GetSprintRequest>
{
    public GetSprintRequestValidator()
    {
        RuleFor(c => c.Id).NotEmpty().Must(BeValidGuid);
    }
}