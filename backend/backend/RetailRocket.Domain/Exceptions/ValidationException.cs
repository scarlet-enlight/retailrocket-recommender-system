namespace RetailRocket.Domain.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string errorMessage) : base(errorMessage) {}
}