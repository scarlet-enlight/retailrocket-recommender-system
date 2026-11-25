namespace RetailRocket.Domain.Exceptions;

public class AuthorizationException : Exception
{
    public AuthorizationException(string errorMessage) : base(errorMessage) {}
}