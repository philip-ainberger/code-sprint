using CodeSprint.Common.Grpc.Tagging;
using FluentValidation;

namespace CodeSprint.Api.Validators.Tagging;

public class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
{
    public CreateTagRequestValidator()
    {
        RuleFor(c => c.Name).NotEmpty().Length(1, 40);
    }
}