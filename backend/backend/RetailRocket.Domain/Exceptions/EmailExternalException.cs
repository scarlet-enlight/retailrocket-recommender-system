namespace RetailRocket.Domain.Exceptions;

public class EmailExternalException : Exception
{
    public EmailExternalException(string errorMessage) : base(errorMessage) {}
}