using FluentValidation;

namespace CodeSprint.Api.Validators;

public abstract class BaseRequestValidator<T> : AbstractValidator<T>
{
    internal static bool BeValidGuid(string arg)
    {
        return !string.IsNullOrEmpty(arg) && Guid.TryParse(arg, out _);
    }
}