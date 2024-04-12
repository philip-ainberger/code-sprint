namespace CodeSprint.Commom.Exceptions;

public class EntityNotFoundException : ApplicationException
{
    public EntityNotFoundException() : base()
    {
    }

    public EntityNotFoundException(string message) : base(message)
    {
    }
}