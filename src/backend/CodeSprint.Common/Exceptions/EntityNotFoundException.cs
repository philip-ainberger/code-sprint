namespace CodeSprint.Common.Exceptions;

public class EntityNotFoundException : ApplicationException
{
    public EntityNotFoundException() : base()
    {
    }

    public EntityNotFoundException(string message) : base(message)
    {
    }
}