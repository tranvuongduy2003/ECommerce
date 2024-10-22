namespace Ordering.Domain.Exceptions;

public class InvalidEntityTypeException : ApplicationException
{
    public InvalidEntityTypeException(string name, string type)
        : base($"Entity \"{name}\" not supported type: {type}")
    {
    }
}