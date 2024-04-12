namespace CodeSprint.Commom.Exceptions
{
    public class MissingValidatorException<TRequest> : ApplicationException
    {
        public MissingValidatorException() 
            : base(string.Format("Could not get/resolve fluent validator for request type '{0}'", nameof(TRequest)))
        {
            
        }
    }
}